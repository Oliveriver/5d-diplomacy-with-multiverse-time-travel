namespace Entities;

public class Support : Order
{
    public Location Midpoint { get; set; } = null!;
    public Location Destination { get; set; } = null!;
}
