using Entities;
using Enums;

namespace Adjudication;

public class OrderTreeEvaluator(World world, List<Order> orders, List<Region> regions, AdjacencyValidator adjacencyValidator)
{
    private readonly World world = world;

    private readonly List<Region> regions = regions;
    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;
    private readonly StrengthCalculator strengthCalculator = new(orders, adjacencyValidator);

    private readonly List<Order> orders = orders;

    private readonly List<Hold> holds = orders.OfType<Hold>().ToList();
    private readonly List<Move> moves = orders.OfType<Move>().Where(m => m.Status != OrderStatus.Invalid).ToList();
    private readonly List<Support> supports = orders.OfType<Support>().Where(s => s.Status != OrderStatus.Invalid).ToList();
    private readonly List<Convoy> convoys = orders.OfType<Convoy>().Where(c => c.Status != OrderStatus.Invalid).ToList();

    public void ApplyEvaluationPass()
    {
        foreach (var support in supports)
        {
            EvaluateSupport(support);
        }

        foreach (var convoy in convoys)
        {
            EvaluateConvoy(convoy);
        }

        UpdateOrderStrengths();

        foreach (var move in moves)
        {
            EvaluateMove(move);
        }

        foreach (var hold in holds)
        {
            EvaluateHold(hold);
        }

        UpdateConvoyPaths();
        UpdateOrderStrengths();
        UpdateSupportsAttackingSelf();
    }

    private void UpdateOrderStrengths()
    {
        foreach (var order in orders)
        {
            strengthCalculator.UpdateOrderStrength(order);
        }
    }

    private void UpdateConvoyPaths()
    {
        var possibleConvoys = convoys.Where(c => c.Status != OrderStatus.Failure).ToList();
        var convoyPathValidator = new ConvoyPathValidator(world, possibleConvoys, regions, adjacencyValidator);
        foreach (var move in moves)
        {
            var newConvoyPath = convoyPathValidator.GetPossibleConvoys(move.Unit!, move.Location, move.Destination);

            var canDirectMove = adjacencyValidator.IsValidDirectMove(move.Unit!, move.Location, move.Destination);
            if (newConvoyPath.Count == 0 && !canDirectMove)
            {
                move.Status = OrderStatus.Failure;
            }

            move.ConvoyPath = newConvoyPath;
        }
    }

    private void UpdateSupportsAttackingSelf()
    {
        foreach (var move in moves)
        {
            if (move.Status == OrderStatus.Success)
            {
                var destinationOrder = orders.FirstOrDefault(o => adjacencyValidator.EqualsOrIsRelated(move.Destination, o.Location));
                if (destinationOrder == null)
                {
                    continue;
                }

                if (destinationOrder is not Move
                    || destinationOrder is Move && destinationOrder.Status is OrderStatus.Invalid or OrderStatus.Failure)
                {
                    foreach (var support in move.Supports)
                    {
                        if (support.Unit!.Owner == destinationOrder.Unit!.Owner)
                        {
                            support.Status = OrderStatus.Failure;
                        }
                    }
                }
            }
        }
    }

    private void EvaluateHold(Hold hold)
    {
        var attackingMoves = moves.Where(m => !m.IsSzykmanHold && adjacencyValidator.EqualsOrIsRelated(m.Destination, hold.Location));
        if (attackingMoves.Any(m => m.Status == OrderStatus.Success))
        {
            hold.Status = OrderStatus.Failure;
        }
        else if (attackingMoves.All(m => m.Status == OrderStatus.Failure))
        {
            hold.Status = OrderStatus.Success;
        }
    }

    private void EvaluateMove(Move move)
    {
        var attackingMoves = moves.Where(m => m != move && !m.IsSzykmanHold && adjacencyValidator.EqualsOrIsRelated(m.Destination, move.Destination));

        var beatsPreventStrength = move.AttackStrength.Min > (attackingMoves.Any() ? attackingMoves.Max(m => m.PreventStrength.Max) : 0);
        var losesToPreventStrength = move.AttackStrength.Max <= (attackingMoves.Any() ? attackingMoves.Min(m => m.PreventStrength.Min) : 0);

        var isPreventedByAllDislodgedConvoys = true;

        foreach (var attackingMove in attackingMoves)
        {
            if (attackingMove.Status != OrderStatus.Failure
                || adjacencyValidator.IsValidDirectMove(attackingMove.Unit!, attackingMove.Location, attackingMove.Destination))
            {
                isPreventedByAllDislodgedConvoys = false;
                break;
            }

            var attackingConvoys = convoys.Where(c => c.Midpoint == attackingMove.Location && c.Destination == attackingMove.Destination);
            foreach (var convoy in attackingConvoys)
            {
                var isDislodged = moves.Any(m => m.Destination == convoy.Location && m.Status == OrderStatus.Success);
                if (!isDislodged)
                {
                    isPreventedByAllDislodgedConvoys = false;
                    break;
                }
            }
        }

        if (isPreventedByAllDislodgedConvoys)
        {
            beatsPreventStrength = true;
            losesToPreventStrength = false;
        }

        if (move.OpposingMove != null)
        {
            var beatsDefendStrength = move.AttackStrength.Min > move.OpposingMove.DefendStrength.Max;
            var losesToDefendStrength = move.AttackStrength.Max <= move.OpposingMove.DefendStrength.Min;

            if (beatsDefendStrength && beatsPreventStrength)
            {
                move.Status = OrderStatus.Success;
            }
            else if (losesToPreventStrength || losesToDefendStrength)
            {
                move.Status = OrderStatus.Failure;
            }
        }
        else
        {
            var destinationOrder = orders.FirstOrDefault(o => adjacencyValidator.EqualsOrIsRelated(o.Location, move.Destination));

            if (destinationOrder is Move destinationMove
                && destinationMove.Unit!.Owner == move.Unit!.Owner
                && destinationMove.Status == OrderStatus.Success
                && destinationMove.OpposingMove != null
                && attackingMoves.All(m => m == destinationMove.OpposingMove))
            {
                move.Status = OrderStatus.Success;
                return;
            }

            var beatsHoldStrength = move.AttackStrength.Min > (destinationOrder?.HoldStrength.Max ?? 0);
            var losesToHoldStrength = move.AttackStrength.Max <= (destinationOrder?.HoldStrength.Min ?? 0);

            if (beatsHoldStrength && beatsPreventStrength)
            {
                move.Status = OrderStatus.Success;
            }
            else if (losesToPreventStrength || losesToHoldStrength)
            {
                move.Status = OrderStatus.Failure;
            }
        }
    }

    private void EvaluateSupport(Support support)
    {
        var attackingMoves = moves.Where(m => !m.IsSzykmanHold && adjacencyValidator.EqualsOrIsRelated(m.Destination, support.Location) && !m.IsSzykmanHold);

        if (!attackingMoves.Any(m => !adjacencyValidator.EqualsOrIsRelated(m.Location, support.Destination)))
        {
            support.Status = OrderStatus.Success;
            return;
        }

        var isAttackedByAllDislodgedConvoys = true;

        foreach (var attackingMove in attackingMoves)
        {
            if (attackingMove.Status != OrderStatus.Failure
                || adjacencyValidator.IsValidDirectMove(attackingMove.Unit!, attackingMove.Location, attackingMove.Destination))
            {
                isAttackedByAllDislodgedConvoys = false;
            }

            var attackingConvoys = convoys.Where(c => c.Midpoint == attackingMove.Location && c.Destination == attackingMove.Destination);
            foreach (var convoy in attackingConvoys)
            {
                var isDislodged = moves.Any(m => m.Destination == convoy.Location && m.Status == OrderStatus.Success);
                if (!isDislodged)
                {
                    isAttackedByAllDislodgedConvoys = false;
                }

                if (convoy.Location == support.Destination)
                {
                    return;
                }
            }
        }

        if (isAttackedByAllDislodgedConvoys)
        {
            support.Status = OrderStatus.Success;
            return;
        }

        if (attackingMoves.Any(m => m.Status == OrderStatus.Success))
        {
            support.Status = OrderStatus.Failure;
            return;
        }

        var opposingMove = attackingMoves.FirstOrDefault(m => !m.IsSzykmanHold && adjacencyValidator.EqualsOrIsRelated(m.Location, support.Destination));

        var isAttackedByUnresolvedConvoy = attackingMoves.Any(m =>
            m.Unit!.Owner != support.Unit!.Owner
            && m.ConvoyPath.Any(c => c.Status == OrderStatus.New));

        if (opposingMove != null || isAttackedByUnresolvedConvoy)
        {
            return;
        }

        if (attackingMoves.Any(m => m.Unit!.Owner != support.Unit!.Owner))
        {
            support.Status = OrderStatus.Failure;
            return;
        }

        support.Status = OrderStatus.Success;
    }

    private void EvaluateConvoy(Convoy convoy)
    {
        var attackingMoves = moves.Where(m => !m.IsSzykmanHold && m.Destination == convoy.Location);

        if (convoy.Status != OrderStatus.Failure && !attackingMoves.Any(m => m.Status != OrderStatus.Failure))
        {
            convoy.Status = OrderStatus.Success;
            return;
        }

        if (attackingMoves.Any(m => m.Status == OrderStatus.Success))
        {
            convoy.Status = OrderStatus.Failure;
        }
    }
}
