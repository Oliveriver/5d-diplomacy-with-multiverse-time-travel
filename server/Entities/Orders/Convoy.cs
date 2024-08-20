using Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public class Convoy : Order
{
    public Location Midpoint { get; set; } = null!;
    public Location Destination { get; set; } = null!;

    [NotMapped]
    public override bool NeedsValidation => Status is OrderStatus.Invalid or OrderStatus.New;

    [NotMapped]
    public override List<Location> TouchedLocations => [Location, Midpoint, Destination];
}
