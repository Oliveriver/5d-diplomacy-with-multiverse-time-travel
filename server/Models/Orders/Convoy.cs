using Enums;

namespace Models;

public class Convoy(OrderStatus status, Unit unit, Location location, Location destination, Location convoyLocation) : Order(status, unit, location)
{
    public Location ConvoyLocation { get; set; } = convoyLocation;
    public Location Destination { get; set; } = destination;
}
