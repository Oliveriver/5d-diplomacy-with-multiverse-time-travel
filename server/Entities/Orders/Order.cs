using Enums;

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
}
