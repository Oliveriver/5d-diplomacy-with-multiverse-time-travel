using Context;
using Entities;
using Enums;
using Exceptions;
using Factories;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Utilities;

namespace Repositories;

public class GameRepository(ILogger<GameRepository> logger, GameContext context, DefaultWorldFactory defaultWorldFactory)
{
    private readonly ILogger<GameRepository> logger = logger;
    private readonly GameContext context = context;
    private readonly DefaultWorldFactory defaultWorldFactory = defaultWorldFactory;
    private readonly Random random = new();

    public async Task<Game> GetGame(int id)
    {
        logger.LogInformation("Fetching game {Id}", id);

        var game = await context.Games.FindAsync(id) ?? throw new GameNotFoundException();
        return game;
    }

    public async Task<(Game game, Nation player)> CreateNormalGame(Nation? player, bool hasStrictAdjacencies)
    {
        logger.LogInformation("Creating game as {Player}", player);

        var defaultWorld = defaultWorldFactory.CreateWorld();

        var chosenPlayer = player ?? GetRandomNation([]);
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

        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var game = await context.Games.FindAsync(id) ?? throw new GameNotFoundException();

        if (game.IsSandbox)
        {
            throw new GameInvalidException("Can't join sandbox game when requesting to join normal game");
        }

        var chosenPlayer = player ?? GetRandomNation(game.Players);
        game.Players.Add(chosenPlayer);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        return (game, chosenPlayer);
    }

    public async Task<Game> JoinSandboxGame(int id)
    {
        logger.LogInformation("Joining sandbox game {Id}", id);

        var game = await context.Games
            .AsNoTracking()
            .SingleOrDefaultAsync(g => g.Id == id) ?? throw new GameNotFoundException();

        return game.IsSandbox
            ? game
            : throw new GameInvalidException("Can't join non-sandbox when requesting to join sandbox game");
    }

    private Nation GetRandomNation(List<Nation> existingNations)
    {
        var availableNations = Constants.Nations.Except(existingNations);
        if (!availableNations.Any())
        {
            throw new GameInvalidException("Can't rejoin full game as random nation");
        }

        var player = availableNations.ElementAt(random.Next(availableNations.Count()));
        logger.LogInformation("Selected random nation {Player}", player);
        return player;
    }
}

