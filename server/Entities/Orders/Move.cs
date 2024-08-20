using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public class Move : Order
{
    public Location Destination { get; set; } = null!;

    [NotMapped]
    public override List<Location> TouchedLocations => [Location, Destination];
}
