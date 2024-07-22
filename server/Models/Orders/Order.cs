using Enums;
using System.Text.Json.Serialization;

namespace Models;

[JsonDerivedType(typeof(Hold), typeDiscriminator: "Hold")]
[JsonDerivedType(typeof(Move), typeDiscriminator: "Move")]
[JsonDerivedType(typeof(Support), typeDiscriminator: "Support")]
[JsonDerivedType(typeof(Convoy), typeDiscriminator: "Convoy")]
[JsonDerivedType(typeof(Build), typeDiscriminator: "Build")]
[JsonDerivedType(typeof(Disband), typeDiscriminator: "Disband")]
public abstract class Order(OrderStatus status, Unit unit, Location location)
{
    public OrderStatus Status { get; set; } = status;
    public Unit Unit { get; set; } = unit;
    public Location Location { get; set; } = location;
}
