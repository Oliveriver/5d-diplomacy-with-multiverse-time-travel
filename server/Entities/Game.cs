using Enums;

namespace Entities;

public class Game
{
    public int Id { get; set; }

    public bool IsSandbox { get; set; }
    public bool HasStrictAdjacencies { get; set; }

    public List<Nation> Players { get; set; } = [];
    public virtual World? World { get; set; }
    public List<Nation> PlayersSubmitted { get; set; } = [];
}
