using Adjudication;
using Context;
using Entities;
using Enums;
using Factories;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

public class WorldRepository(ILogger<WorldRepository> logger, GameContext context, MapFactory mapFactory, DefaultWorldFactory defaultWorldFactory)
{
    private readonly ILogger<WorldRepository> logger = logger;
    private readonly GameContext context = context;
    private readonly MapFactory mapFactory = mapFactory;
    private readonly DefaultWorldFactory defaultWorldFactory = defaultWorldFactory;

    public async Task<World> GetWorld(int gameId)
    {
        logger.LogInformation("Querying world for game {GameId}", gameId);

        var world = await context.Worlds
            .Include(w => w.Boards).ThenInclude(b => b.Centres)
            .Include(w => w.Boards).ThenInclude(b => b.Units)
            .Include(w => w.Orders).ThenInclude(o => o.Unit)
            .AsSplitQuery() // TODO wrap database operations in shared lock to prevent concurrency issues
            .FirstOrDefaultAsync(w => w.GameId == gameId)
            ?? throw new KeyNotFoundException("World not found");

        return world;
    }

    public async Task<int> GetIteration(int gameId)
    {
        logger.LogInformation("Querying iteration for game {GameId}", gameId);

        var world = await context.Worlds.FirstOrDefaultAsync(w => w.GameId == gameId)
            ?? throw new KeyNotFoundException("World not found");

        return world.Iteration;
    }

    public async Task AddOrders(int gameId, Nation[] players, List<Order> orders)
    {
        logger.LogInformation("Submitting orders for game {GameId}", gameId);

        var game = await context.Games.FindAsync(gameId)
            ?? throw new KeyNotFoundException("Game not found");
        var world = await GetWorld(gameId);
        world.Orders.AddRange(orders);

        var newPlayersSubmitted = players.Where(p => !game.PlayersSubmitted.Contains(p));
        game.PlayersSubmitted = [.. game.PlayersSubmitted, .. newPlayersSubmitted];

        if (world.LivingPlayers.Count <= game.PlayersSubmitted.Count)
        {
            logger.LogInformation("Adjudicating game {GameId}", gameId);

            game.PlayersSubmitted = [];

            world.Iteration++;
            var adjudicator = new Adjudicator(world, game.HasStrictAdjacencies, mapFactory, defaultWorldFactory);
            adjudicator.Adjudicate();

            logger.LogInformation("Adjudicated game {GameId}", gameId);
        }

        await context.SaveChangesAsync();
    }
}
