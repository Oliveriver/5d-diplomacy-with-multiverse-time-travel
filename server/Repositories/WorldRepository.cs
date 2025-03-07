using Adjudication;
using Context;
using Entities;
using Enums;
using Factories;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

public class WorldRepository(ILogger<WorldRepository> logger, GameContext context, RegionMapFactory regionMapFactory, DefaultWorldFactory defaultWorldFactory)
{
    private readonly ILogger<WorldRepository> logger = logger;
    private readonly GameContext context = context;
    private readonly RegionMapFactory regionMapFactory = regionMapFactory;
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
        // TODO add a concurrency lock to prevent race conditions

        logger.LogInformation("Submitting orders for game {GameId}", gameId);

        var game = await context.Games.FindAsync(gameId)
            ?? throw new KeyNotFoundException("Game not found");

        if (game.PlayersSubmitted.Intersect(players).Any())
        {
            logger.LogInformation("Found existing submission for players {Players}, ignoring new submission", players);
            return;
        }

        game.PlayersSubmitted = [.. game.PlayersSubmitted, .. players];
        await context.SaveChangesAsync();

        var world = await GetWorld(gameId);
        world.Orders.AddRange(orders);

        if (world.LivingPlayers.Count <= game.PlayersSubmitted.Count)
        {
            logger.LogInformation("Adjudicating game {GameId}", gameId);

            game.PlayersSubmitted = [];

            var adjudicator = new Adjudicator(world, game.HasStrictAdjacencies, regionMapFactory, defaultWorldFactory);
            adjudicator.Adjudicate();
            world.Iteration++;

            logger.LogInformation("Adjudicated game {GameId}", gameId);
        }

        await context.SaveChangesAsync();
    }
}
