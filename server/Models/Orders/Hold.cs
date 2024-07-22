using Enums;

namespace Models;

public class Hold(OrderStatus status, Unit unit, Location location) : Order(status, unit, location)
{
}
