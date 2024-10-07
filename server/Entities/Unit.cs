using Enums;

namespace Entities;

public class Unit
{
    public int Id { get; set; }

    public int BoardId { get; set; }
    public virtual Board Board { get; set; } = null!;
    public virtual List<Order> Orders { get; set; } = [];

    public Location Location { get; set; } = null!;
    public Nation Owner { get; set; }
    public UnitType Type { get; set; }
    public bool MustRetreat { get; set; }

    public Unit Clone() => new()
    {
        Owner = Owner,
        Type = Type,
        MustRetreat = MustRetreat,
        Location = new()
        {
            Timeline = Location.Timeline,
            Year = Location.Year,
            Phase = Location.Phase,
            RegionId = Location.RegionId,
        }
    };

    public override string ToString() => $"{Owner} {Type} at {Location}";
}

