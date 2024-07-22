namespace Entities;

public class Convoy : Order
{
    public Location Midpoint { get; set; } = null!;
    public Location Destination { get; set; } = null!;
}
