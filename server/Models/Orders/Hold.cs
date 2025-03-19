using Enums;

namespace Models;

public class Hold(OrderStatus status, Unit unit, Location location) : Order(status, unit, location)
{
    public override string ToString() => $"Hold {Location}: {Status}";
}
