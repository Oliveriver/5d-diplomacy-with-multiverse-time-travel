using System.Net.WebSockets;
using Adjudication;
using Enums;
using Entities;
using System.Text.Json;
using Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Controllers;

public class WebSocketConnectionManager
{
    private readonly List<Connection> connections = [];
    private readonly EntityMapper entityMapper = new();
    private readonly ILogger<WebSocketConnectionManager> logger;

    private readonly JsonSerializerOptions jsonSerializerOptions;

    public WebSocketConnectionManager(ILogger<WebSocketConnectionManager> logger, IOptions<JsonOptions> jsonOptions)
    {
        this.logger = logger;
        Adjudicator.Adjudicated += SendUpdatedWorldData;

        jsonSerializerOptions = jsonOptions.Value.JsonSerializerOptions;
    }

    public void AddConnection(WebSocket socket, TaskCompletionSource<object> socketFinishedTcs, int gameId, Nation player)
    {
        lock (connections)
        {
            var connection = new Connection(socket, socketFinishedTcs, gameId, player);
            connections.Add(connection);
            ListenForClose(connection);
        }
    }

    private void SendUpdatedWorldData(World world)
    {
        lock (connections)
        {
            logger.LogInformation("Sending world update for game {GameId} to websocket listeners", world.GameId);
            foreach (var connection in connections)
            {
                if (connection.Socket.CloseStatus != null)
                {
                    CleanupConnection(connection);
                    continue;
                }

                if (connection.GameId == world.GameId)
                {
                    var data = JsonSerializer.SerializeToUtf8Bytes(entityMapper.MapWorld(world, connection.Player), jsonSerializerOptions);
                    try
                    {
                        connection.Socket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch (WebSocketException ex)
                    {
                        logger.LogError("Exception when sending world update for game {GameId} to player {Player}: {Exception}", world.GameId, connection.Player, ex);
                    }
                }
            }
        }
    }

    private async void ListenForClose(Connection connection)
    {
        try
        {
            byte[] buff = [];
            await connection.Socket.ReceiveAsync(buff, CancellationToken.None);
        }
        catch (WebSocketException) { }
        finally
        {
            lock (connections)
            {
                CleanupConnection(connection);
            }
        }
    }

    private void CleanupConnection(Connection connection)
    {
        logger.LogInformation("Removing connection for player {Player} in game {GameId}", connection.Player, connection.GameId);
        connections.Remove(connection);
        connection.Socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        connection.SocketFinishedTcs.SetResult("");
    }

    private readonly struct Connection(WebSocket socket, TaskCompletionSource<object> socketFinishedTcs, int gameId, Nation player)
    {
        public readonly WebSocket Socket = socket;
        public readonly TaskCompletionSource<object> SocketFinishedTcs = socketFinishedTcs;
        public readonly int GameId = gameId;
        public readonly Nation Player = player;
    }
}
