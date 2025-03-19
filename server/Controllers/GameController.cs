using System.Text.Json;
using Enums;
using Exceptions;
using Mappers;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repositories;
using Utilities;

namespace Controllers;

[ApiController]
[Route("game")]
public class GameController(
    ILogger<GameController> logger,
    EntityMapper entityMapper,
    GameRepository gameRepository,
    WorldRepository worldRepository) : ControllerBase
{
    private readonly ILogger<GameController> logger = logger;
    private readonly EntityMapper entityMapper = entityMapper;
    private readonly GameRepository gameRepository = gameRepository;
    private readonly WorldRepository worldRepository = worldRepository;

    [HttpPost]
    public async Task<ActionResult<Game>> CreateGame([FromBody] GameCreationRequest request)
    {
        var isSandbox = request.IsSandbox;
        var player = request.Player;
        var hasStrictAdjacencies = request.HasStrictAdjacencies;

        if (isSandbox && player != null)
        {
            logger.LogError("Attempted to create sandbox game with nation {Player} specified", player);
            return BadRequest("Sandbox must be created with no player specified");
        }

        if (isSandbox)
        {
            var game = await gameRepository.CreateSandboxGame(hasStrictAdjacencies);
            return Ok(new Game(game.Id, game.HasStrictAdjacencies));
        }
        else
        {
            var (game, chosenPlayer) = await gameRepository.CreateNormalGame(player, hasStrictAdjacencies);
            return Ok(new Game(game.Id, game.HasStrictAdjacencies, chosenPlayer));
        }
    }

    [HttpPut]
    [Route("{gameId}/players")]
    public async Task<ActionResult<Game>> JoinGame([FromRoute] int gameId, [FromBody] GameJoinRequest request)
    {
        var player = request.Player;
        var isSandbox = request.IsSandbox;

        try
        {
            if (isSandbox)
            {
                var game = await gameRepository.JoinSandboxGame(gameId);
                return Ok(new Game(game.Id, game.HasStrictAdjacencies));
            }
            else
            {
                var (game, chosenPlayer) = await gameRepository.JoinNormalGame(gameId, player);
                return Ok(new Game(game.Id, game.HasStrictAdjacencies, chosenPlayer));
            }
        }
        catch (GameNotFoundException)
        {
            logger.LogError("Attempted to join non-existent game {GameId}", gameId);
            return NotFound($"No game with ID {gameId} found");
        }
        catch (GameInvalidException error)
        {
            logger.LogWarning("Failed to join game {GameId}", gameId);
            return BadRequest(error.Message);
        }
    }

    [HttpPost]
    [Route("load")]
    public async Task<ActionResult<Game>> LoadGame([FromForm] GameLoadRequest request)
    {
        var isSandbox = request.IsSandbox;
        var player = request.Player;

        if (isSandbox && player != null)
        {
            logger.LogError("Attempted to create sandbox game with nation {Player} specified", player);
            return BadRequest("Sandbox must be created with no player specified");
        }

        SaveFile? saveFile;
        try
        {
            await using var stream = request.File.OpenReadStream();
            saveFile = await JsonSerializer.DeserializeAsync<SaveFile>(stream, Constants.JsonOptions);
        }
        catch (JsonException)
        {
            return BadRequest("Failed to deserialise uploaded JSON file");
        }

        if (saveFile == null)
        {
            return BadRequest("Uploaded JSON file was deserialised to null");
        }

        const int MaxIteration = 200 * 3;
        if (saveFile.Iteration > MaxIteration)
        {
            return BadRequest($"Uploaded JSON file has {saveFile.Iteration} iterations, maximum for loading is {MaxIteration}.");
        }

        foreach (var order in saveFile.Orders)
        {
            order.Status = order.IsRetreat() ? OrderStatus.RetreatNew : OrderStatus.New;
        }

        async Task AddOrders(int gameId)
        {
            var processedOrders = new List<Order>(saveFile.Orders.Length);
            var ordersByBoardLocation = saveFile.Orders.ToLookup(o => (o.Location.Timeline, o.Location.Year, o.Location.Phase, IsRetreat: o.IsRetreat()));

            await worldRepository.AddOrdersForMultipleIterations(gameId, saveFile.Iteration, world =>
            {
                var hasRetreats = world.HasRetreats();
                var currentOrders = world.ActiveBoards.SelectMany(b => ordersByBoardLocation[(b.Timeline, b.Year, b.Phase, hasRetreats)]).ToList();
                processedOrders.AddRange(currentOrders);
                return currentOrders;
            });

            var unprocessedOrders = saveFile.Orders.Except(processedOrders).ToList();
            if (unprocessedOrders.Count > 0)
            {
                logger.LogWarning(
                    "{Count} unprocessed orders during load\n{UnprocessedOrders}",
                    unprocessedOrders.Count,
                    string.Join("\n", unprocessedOrders));
            }
        }

        var hasStrictAdjacencies = saveFile.HasStrictAdjacencies;

        if (isSandbox)
        {
            var game = await gameRepository.CreateSandboxGame(hasStrictAdjacencies);
            await AddOrders(game.Id);
            return Ok(new Game(game.Id, game.HasStrictAdjacencies));
        }
        else
        {
            var (game, chosenPlayer) = await gameRepository.CreateNormalGame(player, hasStrictAdjacencies);
            await AddOrders(game.Id);
            return Ok(new Game(game.Id, game.HasStrictAdjacencies, chosenPlayer));
        }
    }

    [HttpGet]
    [Route("{gameId}/players/submitted")]
    public async Task<ActionResult<List<Nation>>> GetPlayersSubmitted([FromRoute] int gameId)
    {
        logger.LogInformation("Fetching submitted players for game {GameId}", gameId);

        try
        {
            var game = await gameRepository.GetGame(gameId);
            return Ok(game.PlayersSubmitted);
        }
        catch (GameNotFoundException)
        {
            logger.LogError("Attempted to find submitted players for non-existent game {GameId}", gameId);
            return NotFound($"No game with ID {gameId} found");
        }
    }

    [HttpGet]
    [Route("{gameId}")]
    public async Task<ActionResult<World>> GetWorld([FromRoute] int gameId, [FromQuery] Nation player)
    {
        logger.LogInformation("Fetching world for game {GameId} as {Player}", gameId, player);

        try
        {
            var world = await worldRepository.GetWorld(gameId);
            return Ok(entityMapper.MapWorld(world, player));
        }
        catch (GameNotFoundException)
        {
            logger.LogWarning("Failed to find world with ID {GameId}", gameId);
            return NotFound($"No world with game ID {gameId} found");
        }
    }

    [HttpGet]
    [Route("{gameId}/iteration")]
    public async Task<ActionResult> GetIteration([FromRoute] int gameId, [FromQuery] Nation player)
    {
        logger.LogInformation("Fetching iteration number for game {GameId} as {Player}", gameId, player);

        try
        {
            var iteration = await worldRepository.GetIteration(gameId);
            return Ok(iteration);
        }
        catch (GameNotFoundException)
        {
            logger.LogWarning("Failed to find world with ID {GameId}", gameId);
            return NotFound($"No world with game ID {gameId} found");
        }
    }

    [HttpPost]
    [Route("{gameId}/orders")]
    public async Task<ActionResult> SubmitOrders([FromRoute] int gameId, [FromBody] OrderSubmissionRequest request)
    {
        var players = request.Players;
        var orders = request.Orders;
        logger.LogInformation("Adding submitted orders as player {Players} for game {GameId}", players, gameId);

        if (players.Length == 0)
        {
            logger.LogError("Failed to submit with empty player list");
            return BadRequest("No players specified");
        }

        try
        {
            await worldRepository.AddOrders(gameId, players, orders);
            return Ok();
        }
        catch (GameNotFoundException)
        {
            logger.LogWarning("Failed to find world with ID {GameId}", gameId);
            return NotFound($"No world with game ID {gameId} found");
        }
    }
}
