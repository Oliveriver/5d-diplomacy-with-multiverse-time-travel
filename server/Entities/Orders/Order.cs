using Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public abstract class Order
{
    public int Id { get; set; }

    public int WorldId { get; set; }
    public virtual World World { get; set; } = null!;

    public OrderStatus Status { get; set; }
    public int? UnitId { get; set; }
    public virtual Unit? Unit { get; set; }
    public Location Location { get; set; } = null!;

    [NotMapped]
    public virtual bool NeedsValidation => Status == OrderStatus.New;

    [NotMapped]
    public virtual List<Location> TouchedLocations => [Location];

    [NotMapped]
    public OrderStrength HoldStrength { get; set; } = new();

    [NotMapped]
    public List<Support> PotentialSupports { get; set; } = [];
}
