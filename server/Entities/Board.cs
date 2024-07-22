using Enums;

namespace Entities;

public class Board
{
    public int Id { get; set; }

    public int WorldId { get; set; }
    public virtual World World { get; set; } = null!;

    public int Timeline { get; set; }
    public int Year { get; set; }
    public Phase Phase { get; set; }
    public List<int> ChildTimelines { get; set; } = null!;
    public virtual List<Centre> Centres { get; set; } = null!;
    public virtual List<Unit> Units { get; set; } = null!;
}
