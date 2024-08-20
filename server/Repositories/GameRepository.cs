using Context;
using Entities;
using Enums;
using Factories;
using Utilities;

namespace Repositories;

public class GameRepository(ILogger<GameRepository> logger, GameContext context, DefaultWorldFactory defaultWorldFactory)
{
    private readonly ILogger<GameRepository> logger = logger;
    private readonly GameContext context = context;
    private readonly DefaultWorldFactory defaultWorldFactory = defaultWorldFactory;
    private readonly Random random = new();

    public async Task<(Game game, Nation player)> CreateNormalGame(Nation? player, bool hasStrictAdjacencies)
    {
        logger.LogInformation("Creating game as {Player}", player);

        var defaultWorld = defaultWorldFactory.CreateWorld();

        var chosenPlayer = player ?? GetRandomNation();
        var game = new Game
        {
            IsSandbox = false,
            HasStrictAdjacencies = hasStrictAdjacencies,
            Players = [chosenPlayer],
            World = defaultWorld,
            PlayersSubmitted = [],
        };

        await context.Games.AddAsync(game);
        await context.SaveChangesAsync();

        return (game, chosenPlayer);
    }

    public async Task<Game> CreateSandboxGame(bool hasStrictAdjacencies)
    {
        logger.LogInformation("Creating sandbox game");

        var defaultWorld = defaultWorldFactory.CreateWorld();

        var game = new Game
        {
            IsSandbox = true,
            HasStrictAdjacencies = hasStrictAdjacencies,
            Players = Constants.Nations,
            World = defaultWorld,
            PlayersSubmitted = [],
        };

        await context.Games.AddAsync(game);
        await context.SaveChangesAsync();

        return game;
    }

    public async Task<(Game game, Nation player)> JoinNormalGame(int id, Nation? player)
    {
        logger.LogInformation("Joining game {Id} as player {Player}", id, player);

        var game = await context.Games.FindAsync(id) ?? throw new KeyNotFoundException("Game not found");

        if (game.IsSandbox)
        {
            throw new InvalidOperationException("Can't join sandbox game when requesting to join normal game");
        }

        if (player == null && game.Players.Count >= Constants.Nations.Count)
        {
            throw new InvalidOperationException("Can't rejoin full game as random nation");
        }

        var chosenPlayer = player ?? GetRandomNation();
        if (!game.Players.Contains(chosenPlayer) && game.Players.Count < Constants.Nations.Count)
        {
            game.Players.Add(chosenPlayer);
            await context.SaveChangesAsync();
        }

        return (game, chosenPlayer);
    }

    public async Task<Game> JoinSandboxGame(int id)
    {
        logger.LogInformation("Joining sandbox game {Id}", id);

        var game = await context.Games.FindAsync(id) ?? throw new KeyNotFoundException("Game not found");

        return game.IsSandbox
            ? game
            : throw new InvalidOperationException("Can't join non-sandbox when requesting to join sandbox game");
    }

    private Nation GetRandomNation()
    {
        var player = Constants.Nations.ElementAt(random.Next(Constants.Nations.Count));
        logger.LogInformation("Selected random nation {Player}", player);
        return player;
    }
}

