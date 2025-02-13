using Adjudication;
using Context;
using Entities;
using Enums;
using Factories;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

public class WorldRepository(ILogger<WorldRepository> logger, GameContext context, MapFactory mapFactory, DefaultWorldFactory defaultWorldFactory, Services.BackgroundTaskQueue backgroundTaskQueue)
{
    private readonly ILogger<WorldRepository> logger = logger;
    private readonly GameContext context = context;
    private readonly MapFactory mapFactory = mapFactory;
    private readonly DefaultWorldFactory defaultWorldFactory = defaultWorldFactory;
    private readonly Services.BackgroundTaskQueue backgroundTaskQueue = backgroundTaskQueue;

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

        await context.SaveChangesAsync();
        //FIXME: game needs a "processing" flag/status column, and we need to prevent races on re-submitted final orders
        if (world.LivingPlayers.Count <= game.PlayersSubmitted.Count)
        {
            backgroundTaskQueue.EnqueueTask(async (sf, ct) =>
            {
                using var services = sf.CreateScope();
                //create a new instance for the background task to rely on, which will have all the services we require.
                var bg_wr = services.ServiceProvider.GetRequiredService<WorldRepository>();
                await bg_wr.BackgroundAdjudicate(gameId);
            });
        }
    }
    private async Task BackgroundAdjudicate(int gameId)
    {
        logger.LogInformation("Adjudicating game {GameId}", gameId);

        var game = await context.Games.FindAsync(gameId)
            ?? throw new KeyNotFoundException("Game not found");
        var world = await GetWorld(gameId);
        game.PlayersSubmitted = [];

        var adjudicator = new Adjudicator(world, game.HasStrictAdjacencies, mapFactory, defaultWorldFactory);
        adjudicator.Adjudicate();
        world.Iteration++;

        logger.LogInformation("Adjudicated game {GameId}", gameId);
        await context.SaveChangesAsync();
    }
}
