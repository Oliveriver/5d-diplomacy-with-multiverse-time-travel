using Context;
using Entities;
using Enums;
using Factories;
using Utils;

namespace Repositories;

public class GameRepository(ILogger<GameRepository> logger, GameContext context, DefaultWorldFactory defaultWorldFactory)
{
    private readonly ILogger<GameRepository> logger = logger;
    private readonly GameContext context = context;
    private readonly DefaultWorldFactory defaultWorldFactory = defaultWorldFactory;
    private readonly Random random = new();

    public async Task<(Game game, Nation player)> CreateNormalGame(Nation? player)
    {
        logger.LogInformation("Creating game as {player}", player);

        var defaultWorld = defaultWorldFactory.CreateWorld();

        var chosenPlayer = player ?? GetRandomNation();
        var game = new Game
        {
            Players = [chosenPlayer],
            World = defaultWorld,
            PlayersSubmitted = [],
        };

        await context.Games.AddAsync(game);
        await context.SaveChangesAsync();

        return (game, chosenPlayer);
    }

    public async Task<Game> CreateSandboxGame()
    {
        logger.LogInformation("Creating sandbox game");

        var defaultWorld = defaultWorldFactory.CreateWorld();

        var game = new Game
        {
            Players = Constants.Nations,
            World = defaultWorld,
            PlayersSubmitted = [],
        };

        await context.Games.AddAsync(game);
        await context.SaveChangesAsync();

        return game;
    }

    public async Task<(Game game, Nation player)> JoinGame(int id, Nation? player)
    {
        logger.LogInformation("Joining game {id} as player {player}", id, player);

        var game = await context.Games.FindAsync(id) ?? throw new KeyNotFoundException("Game not found");

        if (player == null && game.Players.Count >= Constants.Nations.Count)
        {
            throw new InvalidOperationException("Can't rejoin full game as random nation");
        }

        var chosenPlayer = player ?? GetRandomNation();
        if (!game.Players.Contains(chosenPlayer) && game.Players.Count < Constants.Nations.Count)
        {
            // Can't use Add due to custom enum conversion
            game.Players = [.. game.Players, chosenPlayer];
            await context.SaveChangesAsync();
        }

        return (game, chosenPlayer);
    }

    private Nation GetRandomNation()
    {
        var player = Constants.Nations.ElementAt(random.Next(Constants.Nations.Count));
        logger.LogInformation("Selected random nation {player}", player);
        return player;
    }
}

