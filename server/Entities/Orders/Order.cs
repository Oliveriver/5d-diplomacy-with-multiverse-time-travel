using Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public abstract class Order
{
    public int Id { get; set; }

    public int WorldId { get; set; }
    public virtual World World { get; set; } = null!;

    public OrderStatus Status { get; set; }
    public int UnitId { get; set; }

    [DeleteBehavior(DeleteBehavior.ClientCascade)]
    public virtual Unit Unit { get; set; } = null!;
    public Location Location { get; set; } = null!;

    [NotMapped]
    public virtual bool NeedsValidation => Status is OrderStatus.New or OrderStatus.RetreatNew;

    public virtual Location[] TouchedLocations() => [Location];

    [NotMapped]
    public OrderStrength HoldStrength { get; set; } = new();

    [NotMapped]
    public List<Support> Supports { get; set; } = [];

    public abstract override string ToString();
}
