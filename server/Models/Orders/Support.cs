using Enums;

namespace Models;

public class Support(OrderStatus status, Unit unit, Location location, Location supportLocation, Location destination) : Order(status, unit, location)
{
    public Location SupportLocation { get; set; } = supportLocation;
    public Location Destination { get; set; } = destination;
}
