using Entities;
using Enums;

namespace Adjudication;

public class MovementEvaluator(
    World world,
    List<Order> activeOrders,
    List<Region> regions,
    AdjacencyValidator adjacencyValidator,
    TouchedOrdersFinder touchedOrdersFinder)
{
    private readonly World world = world;

    private readonly List<Region> regions = regions;
    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;
    private readonly TouchedOrdersFinder touchedOrdersFinder = touchedOrdersFinder;
    private readonly CycleFinder cycleFinder = new(adjacencyValidator);

    public void EvaluateMovements()
    {
        IdentifyHeadToHeadBattles();
        LinkSupports();

        var initialEvaluator = new OrderTreeEvaluator(world, activeOrders, regions, adjacencyValidator);
        initialEvaluator.ApplyEvaluationPass();

        var unresolvedMoves = activeOrders.OfType<Move>().Where(m => m.Status == OrderStatus.New).ToList();
        var unresolvedNonMoves = activeOrders.Where(o => o is not Move && o.Status == OrderStatus.New).ToList();

        if (unresolvedNonMoves.Count > 0 && unresolvedMoves.Count == 0)
        {
            initialEvaluator.ApplyEvaluationPass();
        }

        while (unresolvedMoves.Count > 0)
        {
            IdentifyDependencies(unresolvedMoves);

            foreach (var move in unresolvedMoves)
            {
                ResolveDependencies(move);
            }

            unresolvedMoves = activeOrders.OfType<Move>().Where(m => m.Status == OrderStatus.New).ToList();
        }

        IdentifyRetreats();
    }

    private void LinkSupports()
    {
        var supports = activeOrders.OfType<Support>().Where(s => s.Status != OrderStatus.Invalid).ToList();

        foreach (var support in supports)
        {
            var supportedOrder = activeOrders.First(o => o.Location == support.Midpoint);
            supportedOrder.Supports.Add(support);
        }
    }

    private void IdentifyHeadToHeadBattles()
    {
        var moves = activeOrders.OfType<Move>().Where(m => m.Status != OrderStatus.Invalid).ToList();

        foreach (var move in moves)
        {
            if (move.ConvoyPath.Count > 0)
            {
                continue;
            }

            var opposingMove = moves.FirstOrDefault(m =>
                m.Status != OrderStatus.Invalid
                && adjacencyValidator.EqualsOrIsRelated(m.Location, move.Destination)
                && adjacencyValidator.EqualsOrIsRelated(m.Destination, move.Location)
                && m.ConvoyPath.Count == 0);
            move.OpposingMove = opposingMove;
        }
    }

    private void IdentifyDependencies(List<Move> unresolvedMoves)
    {
        foreach (var move in unresolvedMoves)
        {
            if (unresolvedMoves.Any(m => m.Dependencies.Contains(move)))
            {
                continue;
            }

            move.Dependencies = touchedOrdersFinder.GetTouchedOrders([move]);
        }
    }

    private void ResolveDependencies(Move move)
    {
        if (move.Status != OrderStatus.New)
        {
            return;
        }

        var treeEvaluator = new OrderTreeEvaluator(world, move.Dependencies, regions, adjacencyValidator);

        var initialStatuses = move.Dependencies.Select(o => o.Status).ToList();
        var newStatuses = new List<OrderStatus>();

        while (!initialStatuses.SequenceEqual(newStatuses))
        {
            initialStatuses = newStatuses;
            treeEvaluator.ApplyEvaluationPass();
            newStatuses = move.Dependencies.Select(o => o.Status).ToList();
        }

        if (move.Status == OrderStatus.New)
        {
            var isConsistentSuccess = TryStatusGuess(move, OrderStatus.Success);

            foreach (var order in move.Dependencies)
            {
                order.Status = initialStatuses[move.Dependencies.IndexOf(order)];
            }

            var isConsistentFailure = TryStatusGuess(move, OrderStatus.Failure);

            foreach (var order in move.Dependencies)
            {
                order.Status = initialStatuses[move.Dependencies.IndexOf(order)];
            }

            if (isConsistentSuccess && !isConsistentFailure)
            {
                TryStatusGuess(move, OrderStatus.Success);
            }
            else if (isConsistentFailure && !isConsistentSuccess)
            {
                TryStatusGuess(move, OrderStatus.Failure);
            }
            else if (!isConsistentSuccess && !isConsistentFailure)
            {
                ResolveConvoyParadox(move.Dependencies);
            }
            else
            {
                var cycle = cycleFinder.GetMoveCycle(move.Dependencies);

                if (cycle.Count > 0)
                {
                    foreach (var cycleMove in cycle)
                    {
                        cycleMove.Status = OrderStatus.Success;
                    }
                }
                else
                {
                    ResolveConvoyParadox(move.Dependencies);
                }
            }
        }
    }

    private bool TryStatusGuess(Move move, OrderStatus status)
    {
        move.Status = status;

        var treeEvaluator = new OrderTreeEvaluator(world, move.Dependencies, regions, adjacencyValidator);
        treeEvaluator.ApplyEvaluationPass();

        var initialGuessStatuses = move.Dependencies.Select(o => o.Status).ToList();
        var newGuessStatuses = new List<OrderStatus>();

        while (!initialGuessStatuses.SequenceEqual(newGuessStatuses))
        {
            initialGuessStatuses = newGuessStatuses;
            treeEvaluator.ApplyEvaluationPass();
            newGuessStatuses = move.Dependencies.Select(o => o.Status).ToList();

            if (move.Status != status)
            {
                return false;
            }
        }

        return move.Status == status;
    }

    private void ResolveConvoyParadox(List<Order> orders)
    {
        var movesViaConvoy = orders.OfType<Move>().Where(m => m.ConvoyPath.Count > 0);
        foreach (var move in movesViaConvoy)
        {
            move.Status = OrderStatus.Failure;
        }
    }

    private void IdentifyRetreats()
    {
        var holds = world.Orders.OfType<Hold>();
        var moves = world.Orders.OfType<Move>();
        var supports = world.Orders.OfType<Support>();
        var convoys = world.Orders.OfType<Convoy>();

        List<Order> stationaryOrders = [.. holds, .. supports, .. convoys, .. moves.Where(m => m.Status is OrderStatus.Failure or OrderStatus.Invalid)];

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
}

