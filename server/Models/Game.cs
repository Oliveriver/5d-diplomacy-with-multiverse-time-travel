using Enums;

namespace Models;

public class Game(int id, bool hasStrictAdjacencies, Nation? player = null)
{
    public int Id { get; set; } = id;
    public bool HasStrictAdjacencies { get; set; } = hasStrictAdjacencies;
    public Nation? Player { get; set; } = player;
}
