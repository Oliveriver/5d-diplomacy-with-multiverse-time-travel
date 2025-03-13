using Enums;
using Exceptions;
using Mappers;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repositories;

namespace Controllers;

[ApiController]
[Route("game")]
public class GameController(
    ILogger<GameController> logger,
    EntityMapper entityMapper,
    ModelMapper modelMapper,
    GameRepository gameRepository,
    WorldRepository worldRepository) : ControllerBase
{
    private readonly ILogger<GameController> logger = logger;
    private readonly EntityMapper entityMapper = entityMapper;
    private readonly ModelMapper modelMapper = modelMapper;
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
            var mappedOrders = new List<Entities.Order>();
            foreach (var order in orders)
            {
                var mappedOrder = await modelMapper.MapOrder(gameId, order);
                mappedOrders.Add(mappedOrder);
            }

            await worldRepository.AddOrders(gameId, players, mappedOrders);
            return Ok();
        }
        catch (GameNotFoundException)
        {
            logger.LogWarning("Failed to find world with ID {GameId}", gameId);
            return NotFound($"No world with game ID {gameId} found");
        }
    }
}
