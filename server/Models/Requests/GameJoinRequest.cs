using Enums;

namespace Models;

public class GameJoinRequest(Nation? player)
{
    public Nation? Player { get; set; } = player;
}
