using Entities;
using Enums;

namespace Adjudication;

public class DependencyCalculator(List<Order> orders, AdjacencyValidator adjacencyValidator)
{
    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;

    private readonly List<Move> moves = [.. orders.OfType<Move>().Where(m => m.Status != OrderStatus.Invalid)];
    private readonly List<Support> supports = [.. orders.OfType<Support>().Where(s => s.Status != OrderStatus.Invalid)];

    public List<Order> GetDependents(Order order)
        => order switch
        {
            Hold hold => GetHoldDependents(hold),
            Move move => GetMoveDependents(move),
            Support support => GetSupportDependents(support),
            Convoy convoy => GetConvoyDependents(convoy),
            _ => [],
        };

    private List<Order> GetHoldDependents(Hold hold)
        => [.. supports.Where(s => s.Destination == hold.Location).Cast<Order>()];

    private List<Order> GetMoveDependents(Move move)
    {
        var destinationMoves = moves.Where(m => adjacencyValidator.EqualsOrIsRelated(m.Location, move.Destination));

        return [.. destinationMoves, .. move.Supports, .. move.ConvoyPath];
    }

    private List<Order> GetSupportDependents(Support support)
    {
        var holdSupports = supports.Where(s => adjacencyValidator.EqualsOrIsRelated(s.Midpoint, support.Location));

        var opposingDirectMoves = moves.Where(m =>
            adjacencyValidator.EqualsOrIsRelated(m.Location, support.Destination)
            && adjacencyValidator.EqualsOrIsRelated(m.Destination, support.Location));

        var opposingConvoyMoves = moves.Where(m =>
            adjacencyValidator.EqualsOrIsRelated(m.Destination, support.Location));

        return [.. holdSupports, .. opposingDirectMoves, .. opposingConvoyMoves];
    }

    private List<Order> GetConvoyDependents(Convoy convoy)
    {
        var attackingMoves = moves.Where(m => m.Destination == convoy.Location);
        var holdSupports = supports.Where(s => s.Destination == convoy.Location);

        return [.. attackingMoves, .. holdSupports];
    }
}
