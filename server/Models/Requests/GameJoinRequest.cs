using Enums;

namespace Models;

public class GameJoinRequest(bool isSandbox, Nation? player)
{
    public bool IsSandbox { get; set; } = isSandbox;
    public Nation? Player { get; set; } = player;
}
