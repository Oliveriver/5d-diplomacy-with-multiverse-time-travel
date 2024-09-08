using Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public class Move : Order
{
    private OrderStatus status;

    public override OrderStatus Status
    {
        get => status;
        set
        {
            status = value;

            if (value == OrderStatus.Failure)
            {
                foreach (var support in Supports)
                {
                    support.Status = OrderStatus.Failure;
                }

                foreach (var convoy in ConvoyPath)
                {
                    convoy.Status = OrderStatus.Failure;
                }
            }
        }
    }

    public Location Destination { get; set; } = null!;

    [NotMapped]
    public override List<Location> TouchedLocations => [Location, Destination];

    [NotMapped]
    public List<Convoy> ConvoyPath { get; set; } = [];

    [NotMapped]
    public Move? OpposingMove { get; set; }

    [NotMapped]
    public List<Order> Dependencies { get; set; } = [];

    [NotMapped]
    public OrderStrength AttackStrength { get; set; } = new();

    [NotMapped]
    public OrderStrength DefendStrength { get; set; } = new();

    [NotMapped]
    public OrderStrength PreventStrength { get; set; } = new();

    public override string ToString() => $"Move {Location} to {Destination}: {Status}";
}
