using Enums;

namespace Models;

public class Build(OrderStatus status, Unit unit, Location location) : Order(status, unit, location)
{
    public override string ToString() => $"Build {Location}: {Status}";
}
