using Enums;

namespace Entities;

public class Unit
{
    public int Id { get; set; }

    public int BoardId { get; set; }
    public virtual Board Board { get; set; } = null!;
    public virtual Order? Order { get; set; }

    public Location Location { get; set; } = null!;
    public Nation Owner { get; set; }
    public UnitType Type { get; set; }
    public bool MustRetreat { get; set; }
}

