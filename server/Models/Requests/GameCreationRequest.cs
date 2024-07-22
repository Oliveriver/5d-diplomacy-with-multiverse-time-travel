using Enums;

namespace Models;

public class GameCreationRequest(bool isSandbox, Nation? player)
{
    public bool IsSandbox { get; set; } = isSandbox;
    public Nation? Player { get; set; } = player;
}
