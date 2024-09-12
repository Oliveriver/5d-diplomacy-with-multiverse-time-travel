using Entities;
using Enums;

namespace Adjudication;

public class OrderResolver(List<Order> orders, AdjacencyValidator adjacencyValidator)
{
    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;

    private readonly List<Move> moves = orders.OfType<Move>().Where(m => m.Status != OrderStatus.Invalid).ToList();

    public void TryResolve(Order order)
    {
        if (order.Status != OrderStatus.New)
        {
            return;
        }

        switch (order)
        {
            case Hold hold:
                {
                    TryResolveHold(hold);
                    break;
                }
            case Move move:
                {
                    TryResolveMove(move);
                    break;
                }
            case Support support:
                {
                    TryResolveSupport(support);
                    break;
                }
            case Convoy convoy:
                {
                    TryResolveConvoy(convoy);
                    break;
                }
            default:
                break;
        }
    }

    private void TryResolveHold(Hold hold)
    {
        var attackingMoves = moves.Where(m =>
            !m.IsSzykmanHold
            && adjacencyValidator.EqualsOrIsRelated(m.Destination, hold.Location));

        if (attackingMoves.Any(m => m.Status == OrderStatus.Success))
        {
            hold.Status = OrderStatus.Failure;
        }
        else if (attackingMoves.All(m => m.Status == OrderStatus.Failure))
        {
            hold.Status = OrderStatus.Success;
        }
    }

    private void TryResolveMove(Move move)
    {
        var competingMoves = moves.Where(m =>
            m != move
            && !m.IsSzykmanHold
            && adjacencyValidator.EqualsOrIsRelated(m.Destination, move.Destination));

        var beatsPreventStrength = move.AttackStrength.Min > competingMoves.Select(m => m.PreventStrength.Max).DefaultIfEmpty(0).Max();
        var losesToPreventStrength = move.AttackStrength.Max <= competingMoves.Select(m => m.PreventStrength.Min).DefaultIfEmpty(0).Max();

        var opposingMove = move.OpposingMove;

        if (opposingMove != null && !opposingMove.IsSzykmanHold)
        {
            var beatsDefendStrength = move.AttackStrength.Min > opposingMove.DefendStrength.Max;
            var losesToDefendStrength = move.AttackStrength.Max <= opposingMove.DefendStrength.Min;

            if (beatsDefendStrength && beatsPreventStrength)
            {
                move.Status = OrderStatus.Success;
            }
            else if (losesToDefendStrength || losesToPreventStrength)
            {
                move.Status = OrderStatus.Failure;
            }

            return;
        }

        var destinationOrder = orders.FirstOrDefault(o => adjacencyValidator.EqualsOrIsRelated(o.Location, move.Destination));

        if (destinationOrder is Move destinationMove
            && destinationMove.Unit!.Owner == move.Unit!.Owner
            && destinationMove.OpposingMove != null
            && competingMoves.All(m => m == destinationMove.OpposingMove))
        {
            if (destinationMove.Status == OrderStatus.Success)
            {
                move.Status = OrderStatus.Success;
            }

            return;
        }

        var beatsHoldStrength = move.AttackStrength.Min > (destinationOrder?.HoldStrength.Max ?? 0);
        var losesToHoldStrength = move.AttackStrength.Max <= (destinationOrder?.HoldStrength.Min ?? 0);

        if (beatsHoldStrength && beatsPreventStrength)
        {
            move.Status = OrderStatus.Success;
        }
        else if (losesToHoldStrength || losesToPreventStrength)
        {
            move.Status = OrderStatus.Failure;
        }
    }

    private void TryResolveSupport(Support support)
    {
        var attackingMoves = moves.Where(m =>
            !m.IsSzykmanHold
            && adjacencyValidator.EqualsOrIsRelated(m.Destination, support.Location));

        if (attackingMoves.All(m =>
            m.Unit!.Owner == support.Unit!.Owner
            || adjacencyValidator.EqualsOrIsRelated(m.Location, support.Destination) && m.Status == OrderStatus.Failure
            || !adjacencyValidator.IsValidDirectMove(m.Unit!, m.Location, m.Destination) && m.ConvoyPath.Count == 0
            ))
        {
            support.Status = OrderStatus.Success;
            return;
        }

        if (attackingMoves.Any(m => m.Status == OrderStatus.Success))
        {
            support.Status = OrderStatus.Failure;
            return;
        }

        if (attackingMoves.Any(m =>
            !adjacencyValidator.IsValidDirectMove(m.Unit!, m.Location, m.Destination)
            && m.ConvoyPath.All(c => c.Status == OrderStatus.Success)))
        {
            support.Status = OrderStatus.Failure;
        }

        if (attackingMoves.Any(m =>
            m.Unit!.Owner != support.Unit!.Owner
            && m.Status == OrderStatus.Failure
            && adjacencyValidator.IsValidDirectMove(m.Unit!, m.Location, m.Destination)
            && !adjacencyValidator.EqualsOrIsRelated(m.Location, support.Destination)
            && m.ConvoyPath.All(c => c.Status != OrderStatus.New && c.CanProvidePath)))
        {
            support.Status = OrderStatus.Failure;
            return;
        }
    }

    private void TryResolveConvoy(Convoy convoy)
    {
        var attackingMoves = moves.Where(m => m.Destination == convoy.Location);

        if (convoy.Status != OrderStatus.Failure && attackingMoves.All(m => m.Status == OrderStatus.Failure))
        {
            convoy.Status = OrderStatus.Success;
            return;
        }

        if (attackingMoves.Any(m => m.Status == OrderStatus.Success))
        {
            convoy.Status = OrderStatus.Failure;
            convoy.CanProvidePath = false;
        }
    }
}
