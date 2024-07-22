using Enums;

namespace Entities;

public class Game
{
    public int Id { get; set; }

    public List<Nation> Players { get; set; } = null!;
    public virtual World? World { get; set; }
    public List<Nation> PlayersSubmitted { get; set; } = null!;
}
