using Enums;

namespace Models;

public class GameCreationRequest(bool isSandbox, Nation? player, bool hasStrictAdjacencies)
{
    public bool IsSandbox { get; set; } = isSandbox;
    public Nation? Player { get; set; } = player;
    public bool HasStrictAdjacencies { get; set; } = hasStrictAdjacencies;
}
