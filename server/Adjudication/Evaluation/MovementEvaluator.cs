using Entities;
using Enums;

namespace Adjudication;

public class MovementEvaluator(World world, List<Order> activeOrders, List<Region> regions, AdjacencyValidator adjacencyValidator)
{
    private readonly World world = world;

    private readonly List<Region> regions = regions;
    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;
    private readonly StrengthCalculator strengthCalculator = new(activeOrders, adjacencyValidator);

    private List<Hold> holds = [];
    private List<Move> moves = [];
    private List<Support> supports = [];
    private List<Convoy> convoys = [];

    public void EvaluateMovements()
    {
        holds = activeOrders.OfType<Hold>().ToList();
        moves = activeOrders.OfType<Move>().Where(m => m.Status != OrderStatus.Invalid).ToList();
        supports = activeOrders.OfType<Support>().Where(s => s.Status != OrderStatus.Invalid).ToList();
        convoys = activeOrders.OfType<Convoy>().Where(c => c.Status != OrderStatus.Invalid).ToList();

        IdentifyHeadToHeadBattles();
        ApplyInitialPass();
        // TODO do the rest
        IdentifyRetreats();
    }

    private void IdentifyHeadToHeadBattles()
    {
        foreach (var move in moves)
        {
            if (move.ConvoyPath.Count > 0)
            {
                continue;
            }

            var opposingMove = moves.FirstOrDefault(m =>
                m.Status != OrderStatus.Invalid
                && m.Location == move.Destination
                && m.Destination == move.Location
                && m.ConvoyPath.Count == 0);
            move.OpposingMove = opposingMove;
        }
    }

    private void ApplyInitialPass()
    {
        foreach (var support in supports)
        {
            var supportedOrder = activeOrders.First(o => o.Location == support.Midpoint);
            supportedOrder.Supports.Add(support);

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
    }

    private void UpdateOrderStrengths()
    {
        foreach (var order in activeOrders)
        {
            strengthCalculator.UpdateOrderStrength(order);
        }
    }

    private void UpdateConvoyPaths()
    {
        var possibleConvoys = convoys.Where(c => c.Status is not OrderStatus.Invalid or OrderStatus.Failure).ToList();
        var convoyPathValidator = new ConvoyPathValidator(possibleConvoys, regions, adjacencyValidator);
        foreach (var move in moves)
        {
            move.ConvoyPath = convoyPathValidator.GetPossibleConvoys(move.Unit!, move.Location, move.Destination);
        }
    }

    private void IdentifyRetreats()
    {
        var holds = world.Orders.OfType<Hold>();
        var moves = world.Orders.OfType<Move>();
        var supports = world.Orders.OfType<Support>();
        var convoys = world.Orders.OfType<Convoy>();

        List<Order> stationaryOrders = [.. holds, .. supports, .. convoys, .. moves.Where(m => m.Status == OrderStatus.Invalid)];

        foreach (var order in activeOrders)
        {
            var isSuccessfulMove = order is Move && order.Status == OrderStatus.Success;
            var mustRetreat = !isSuccessfulMove && activeOrders.Any(o =>
                o is Move m
                && adjacencyValidator.EqualsOrIsRelated(m.Destination, order.Location)
                && m.Status == OrderStatus.Success);

            if (!mustRetreat)
            {
                continue;
            }

            var unit = order.Unit!;

            var escapeRoutes = adjacencyValidator.GetAdjacentRegions(unit);

            foreach (var escapeRoute in escapeRoutes)
            {
                var successfulHold = stationaryOrders.FirstOrDefault(o => adjacencyValidator.EqualsOrIsRelated(o.Location, escapeRoute));

                var incomingMoves = moves.Where(m => adjacencyValidator.EqualsOrIsRelated(m.Destination, escapeRoute));

                var successfulIncomingMove = incomingMoves.FirstOrDefault(m => m.Status == OrderStatus.Success);
                var bouncedIncomingMoves = incomingMoves
                    .Where(m => m.Status == OrderStatus.Failure && m.ConvoyPath.All(c => c.Status == OrderStatus.Success));

                var isDestinationOccupied = successfulHold != null || successfulIncomingMove != null;
                var hadBounceInDestination = bouncedIncomingMoves.Count() >= 2;
                var hasOpposingMove = moves.Any(m =>
                    m.Status == OrderStatus.Success
                    && adjacencyValidator.EqualsOrIsRelated(m.Location, escapeRoute)
                    && adjacencyValidator.EqualsOrIsRelated(m.Destination, unit.Location)
                    && m.ConvoyPath.Count == 0);

                if (!isDestinationOccupied && !hadBounceInDestination && !hasOpposingMove)
                {
                    unit.MustRetreat = true;
                    continue;
                }
            }

            var disband = new Disband
            {
                Status = OrderStatus.Retreat,
                Unit = unit,
                UnitId = unit.Id,
                Location = unit.Location,
            };
            world.Orders.Add(disband);
        }
    }

    private void EvaluateHold(Hold hold)
    {
        var attackingMoves = moves.Where(m => adjacencyValidator.EqualsOrIsRelated(m.Destination, hold.Location));
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
        var attackingMoves = moves.Where(m => m != move && adjacencyValidator.EqualsOrIsRelated(m.Destination, move.Destination));

        var beatsPreventStrength = move.AttackStrength.Min > (attackingMoves.Any() ? attackingMoves.Max(m => m.PreventStrength.Max) : 0);
        var losesToPreventStrength = move.AttackStrength.Max <= (attackingMoves.Any() ? attackingMoves.Min(m => m.PreventStrength.Min) : 0);

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
            var destinationOrder = activeOrders.FirstOrDefault(o => adjacencyValidator.EqualsOrIsRelated(o.Location, move.Destination));

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
        var supportedOrder = activeOrders.First(o => o.Location == support.Midpoint);

        var attackingMoves = moves.Where(m => adjacencyValidator.EqualsOrIsRelated(m.Destination, support.Location));
        var opposingMove = attackingMoves.FirstOrDefault(m => adjacencyValidator.EqualsOrIsRelated(m.Location, support.Destination));

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
        var attackingMoves = moves.Where(m => m.Destination == convoy.Location);

        if (!attackingMoves.Any())
        {
            convoy.Status = OrderStatus.Success;
            return;
        }

        foreach (var attackingMove in attackingMoves)
        {
            if (attackingMove.Status == OrderStatus.Success)
            {
                convoy.Status = OrderStatus.Failure;
                return;
            }
        }
    }
}

