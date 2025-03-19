using Enums;

namespace Models;

public class Move(OrderStatus status, Unit unit, Location location, Location destination) : Order(status, unit, location)
{
    public Location Destination { get; set; } = destination;
    public override string ToString() => $"Move {Location} to {Destination}: {Status}";
}
