using Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public class Convoy : Order
{
    public Location Midpoint { get; set; } = null!;
    public Location Destination { get; set; } = null!;

    [NotMapped]
    public override bool NeedsValidation => Status is OrderStatus.Invalid or OrderStatus.New or OrderStatus.RetreatNew;

    public override Location[] TouchedLocations() => [Location, Midpoint, Destination];

    [NotMapped]
    public bool CanProvidePath { get; set; } = true;

    public override string ToString() => $"Convoy {Location} from {Midpoint} to {Destination}: {Status}";
}
