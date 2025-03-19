using Enums;

namespace Models;

public class Disband(OrderStatus status, Unit unit, Location location) : Order(status, unit, location)
{
    public override string ToString() => $"Disband {Location}: {Status}";
}
