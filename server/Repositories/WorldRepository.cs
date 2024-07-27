using Adjudication;
using Context;
using Entities;
using Enums;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace Repositories;

public class WorldRepository(ILogger<WorldRepository> logger, GameContext context, Adjudicator adjudicator)
{
    private readonly ILogger<WorldRepository> logger = logger;
    private readonly GameContext context = context;
    private readonly Adjudicator adjudicator = adjudicator;

    public async Task<World> GetWorld(int gameId)
    {
        logger.LogInformation("Querying world for game {GameId}", gameId);

        var world = await context.Worlds
            .Include(w => w.Boards).ThenInclude(b => b.Centres)
            .Include(w => w.Boards).ThenInclude(b => b.Units)
            .Include(w => w.Orders).ThenInclude(o => o.Unit)
            .FirstOrDefaultAsync(w => w.GameId == gameId)
            ?? throw new KeyNotFoundException("World not found");

        return world;
    }

    public async Task AddOrders(int gameId, Nation[] players, IEnumerable<Order> orders)
    {
        logger.LogInformation("Submitting orders for game {GameId}", gameId);

        var game = await context.Games.FindAsync(gameId)
            ?? throw new KeyNotFoundException("Game not found");
        var world = await GetWorld(gameId);
        world.Orders.AddRange(orders);

        var newPlayersSubmitted = players.Where(p => !game.PlayersSubmitted.Contains(p));
        game.PlayersSubmitted = [.. game.PlayersSubmitted, .. newPlayersSubmitted];

        if (game.PlayersSubmitted.Count == Constants.Nations.Count)
        {
            logger.LogInformation("Adjudicating game {GameId}", gameId);

            game.PlayersSubmitted = [];

            var map = await context.Regions
                .AsNoTracking()
                .Include(r => r.Connections)
                .ToListAsync();

            adjudicator.Adjudicate(world, map);
            world.Iteration++;

            logger.LogInformation("Adjudicated game {GameId}", gameId);
        }

        await context.SaveChangesAsync();
    }
}
