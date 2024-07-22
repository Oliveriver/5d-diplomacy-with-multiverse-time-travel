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

        if (isSandbox && player != null)
        {
            logger.LogError("Attempted to create sandbox game with nation {player} specified", player);
            return BadRequest("Sandbox must be created with no player specified");
        }

        if (isSandbox)
        {
            var game = await gameRepository.CreateSandboxGame();
            return Ok(new Game(game.Id));
        }
        else
        {
            var (game, chosenPlayer) = await gameRepository.CreateNormalGame(player);
            return Ok(new Game(game.Id, chosenPlayer));
        }
    }

    [HttpPut]
    [Route("{gameId}/players")]
    public async Task<ActionResult<Game>> JoinGame([FromRoute] int gameId, [FromBody] GameJoinRequest request)
    {
        var player = request.Player;
        logger.LogInformation("Requesting to join game {gameId} as {player}", gameId, player);

        try
        {
            var (game, chosenPlayer) = await gameRepository.JoinGame(gameId, player);
            return Ok(new Game(game.Id, chosenPlayer));
        }
        catch (KeyNotFoundException)
        {
            logger.LogError("Attempted to join non-existent game {gameId}", gameId);
            return NotFound($"No game with ID {gameId} found");
        }
        catch (InvalidOperationException)
        {
            logger.LogWarning("Failed to join game {gameId}", gameId);
            return BadRequest("Unable to join in-progress game as random nation");
        }
    }

    [HttpGet]
    [Route("{gameId}")]
    public async Task<ActionResult<World>> GetWorld([FromRoute] int gameId)
    {
        logger.LogInformation("Fetching world for game {gameId}", gameId);

        try
        {
            var world = await worldRepository.GetWorld(gameId);
            return Ok(entityMapper.MapWorld(world));
        }
        catch (KeyNotFoundException)
        {
            logger.LogWarning("Failed to find world with ID {gameId}", gameId);
            return NotFound($"No world with game ID {gameId} found");
        }
    }

    [HttpPost]
    [Route("{gameId}/orders")]
    public async Task<ActionResult> SubmitOrders([FromRoute] int gameId, [FromBody] OrderSubmissionRequest request)
    {
        var players = request.Players;
        var orders = request.Orders;
        logger.LogInformation("Adding submitted orders as player {players} for game {gameId}", players, gameId);

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
        catch (KeyNotFoundException)
        {
            logger.LogWarning("Failed to find world with ID {gameId}", gameId);
            return NotFound($"No world with game ID {gameId} found");
        }
    }
}
