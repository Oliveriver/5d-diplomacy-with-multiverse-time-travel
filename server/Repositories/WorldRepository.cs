using Adjudication;
using Context;
using Entities;
using Enums;
using Microsoft.EntityFrameworkCore;
using Utilities;

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

        var activeBoards = world.Boards
            .GroupBy(b => b.Timeline)
            .Select(t => t.MaxBy(b => 3 * b.Year + (int)b.Phase));
        var livingPlayers = Constants.Nations
            .Where(n => activeBoards.Any(b => b?.Centres.Any(c => c.Owner == n) ?? false));

        if (livingPlayers.Count() <= game.PlayersSubmitted.Count)
        {
            logger.LogInformation("Adjudicating game {GameId}", gameId);

            game.PlayersSubmitted = [];

            var regions = await context.Regions
                .Include(r => r.Connections).ThenInclude(c => c.Regions)
                .ToListAsync();

            adjudicator.Adjudicate(world, regions, game.HasStrictAdjacencies);

            logger.LogInformation("Adjudicated game {GameId}", gameId);
        }

        await context.SaveChangesAsync();
    }
}
