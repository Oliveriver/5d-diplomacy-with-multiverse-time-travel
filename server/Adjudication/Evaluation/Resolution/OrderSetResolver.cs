using Entities;
using Enums;

namespace Adjudication;

public class OrderSetResolver(World world, List<Order> orders, RegionMap regionMap, AdjacencyValidator adjacencyValidator)
{
    private readonly World world = world;

    private readonly RegionMap regionMap = regionMap;

    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;
    private readonly OrderResolver orderResolver = new(orders, adjacencyValidator);
    private readonly DependencyCalculator dependencyCalculator = new(orders, adjacencyValidator);
    private readonly StrengthCalculator strengthCalculator = new(orders, adjacencyValidator);

    private readonly List<Move> moves = [.. orders.OfType<Move>().Where(m => m.Status != OrderStatus.Invalid)];
    private readonly List<Support> supports = [.. orders.OfType<Support>().Where(s => s.Status != OrderStatus.Invalid)];
    private readonly List<Convoy> convoys = [.. orders.OfType<Convoy>().Where(c => c.Status != OrderStatus.Invalid)];

    public void RunResolutionAlgorithm()
    {
        ApplyResolutionPass();

        var unresolvedOrders = orders.Where(o => o.Status == OrderStatus.New).ToList();

        while (unresolvedOrders.Count > 0)
        {
            foreach (var order in unresolvedOrders)
            {
                RecursivelyResolve(order, []);
            }

            var newUnresolvedOrders = orders.Where(o => o.Status == OrderStatus.New).ToList();
            if (newUnresolvedOrders.SequenceEqual(unresolvedOrders))
            {
                break;
            }

            unresolvedOrders = newUnresolvedOrders;
        }

        UpdateDependentOrders();
    }

    private void ApplyResolutionPass()
    {
        UpdateConvoyPaths();
        IdentifyHeadToHeadBattles();
        UpdateOrderStrengths();
        UpdateSelfAttackingSupports();

        var initialStatuses = orders.Select(o => o.Status).ToList();
        var newStatuses = new List<OrderStatus>();

        while (!initialStatuses.SequenceEqual(newStatuses))
        {
            initialStatuses = newStatuses;

            foreach (var order in orders)
            {
                orderResolver.TryResolve(order);
            }

            UpdateConvoyPaths();
            IdentifyHeadToHeadBattles();
            UpdateOrderStrengths();
            UpdateSelfAttackingSupports();

            newStatuses = [.. orders.Select(o => o.Status)];
        }
    }

    private void IdentifyHeadToHeadBattles()
    {
        var moves = orders.OfType<Move>().Where(m => m.Status != OrderStatus.Invalid).ToList();

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

    private void UpdateOrderStrengths()
    {
        foreach (var order in orders)
        {
            strengthCalculator.UpdateOrderStrength(order);
        }
    }

    private void UpdateConvoyPaths()
    {
        var possibleConvoys = convoys.Where(c => c.CanProvidePath).ToList();
        var convoyPathValidator = new ConvoyPathValidator(world, possibleConvoys, regionMap, adjacencyValidator);
        foreach (var move in moves)
        {
            var newConvoyPath = convoyPathValidator.GetPossibleConvoys(move.Unit, move.Location, move.Destination);

            var canDirectMove = adjacencyValidator.IsValidDirectMove(move.Unit, move.Location, move.Destination);
            if (newConvoyPath.Count == 0 && !canDirectMove)
            {
                move.Status = OrderStatus.Failure;
            }

            move.ConvoyPath = newConvoyPath;
        }
    }

    private void UpdateDependentOrders()
    {
        foreach (var support in supports)
        {
            var supportedOrder = orders.FirstOrDefault(o => adjacencyValidator.EqualsOrIsRelated(o.Location, support.Midpoint));
            if (supportedOrder is Hold or Move && supportedOrder.Status == OrderStatus.Failure)
            {
                support.Status = OrderStatus.Failure;
            }
        }

        foreach (var convoy in convoys)
        {
            var convoyedMove = orders.FirstOrDefault(o => adjacencyValidator.EqualsOrIsRelated(o.Location, convoy.Midpoint));
            if (convoyedMove?.Status == OrderStatus.Failure)
            {
                convoy.Status = OrderStatus.Failure;
            }
        }
    }

    private void UpdateSelfAttackingSupports()
    {
        foreach (var support in supports)
        {
            if (support.Midpoint == support.Destination)
            {
                continue;
            }

            var destinationOrder = orders.FirstOrDefault(o => adjacencyValidator.EqualsOrIsRelated(o.Location, support.Destination));

            if (destinationOrder == null || destinationOrder.Unit.Owner != support.Unit.Owner)
            {
                continue;
            }

            if (destinationOrder is not Move
                || destinationOrder is Move && destinationOrder.Status is OrderStatus.Invalid or OrderStatus.Failure)
            {
                support.Status = OrderStatus.Failure;
            }
        }
    }

    private void RecursivelyResolve(Order order, List<Order> currentStack)
    {
        if (currentStack.Contains(order))
        {
            ApplyBackupRule(currentStack);
            return;
        }

        ApplyResolutionPass();

        if (order is not Move && order.Status != OrderStatus.New || order is Move && order.Status == OrderStatus.Success)
        {
            return;
        }

        var dependents = dependencyCalculator.GetDependents(order);

        currentStack.Add(order);

        foreach (var dependent in dependents)
        {
            RecursivelyResolve(dependent, currentStack);
        }

        currentStack.Remove(order);
    }

    private void ApplyBackupRule(List<Order> currentStack)
    {
        var guessedOrder = currentStack[0];

        var isConsistentSuccess = TryGuessStatus(guessedOrder, OrderStatus.Success);
        var isConsistentFailure = TryGuessStatus(guessedOrder, OrderStatus.Failure);

        if (isConsistentSuccess && !isConsistentFailure)
        {
            guessedOrder.Status = OrderStatus.Success;
            ApplyResolutionPass();
        }
        else if (isConsistentFailure && !isConsistentSuccess)
        {
            guessedOrder.Status = OrderStatus.Failure;
            ApplyResolutionPass();
        }
        else if (!isConsistentSuccess && !isConsistentFailure)
        {
            ApplySzykmanRule(currentStack);
        }
        else
        {
            var isCycle = true;
            var moves = currentStack.OfType<Move>().ToList();

            foreach (var move in moves)
            {
                var nextMove = moves.FirstOrDefault(m =>
                    adjacencyValidator.EqualsOrIsRelated(m.Location, move.Destination));

                if (nextMove == null)
                {
                    isCycle = false;
                    break;
                }
            }

            if (isCycle)
            {
                guessedOrder.Status = OrderStatus.Success;
                ApplyResolutionPass();
            }
            else
            {
                ApplySzykmanRule(currentStack);
            }
        }
    }

    private bool TryGuessStatus(Order order, OrderStatus status)
    {
        var initialStatuses = orders.Select(o => o.Status).ToList();

        order.Status = status;
        ApplyResolutionPass();

        var isConsistent = order.Status == status;

        for (var i = 0; i < orders.Count; i++)
        {
            orders[i].Status = initialStatuses[i];
        }

        return isConsistent;
    }

    private void ApplySzykmanRule(List<Order> currentStack)
    {
        foreach (var convoy in currentStack.OfType<Convoy>())
        {
            convoy.Status = OrderStatus.Failure;
            convoy.CanProvidePath = false;
        }

        ApplyResolutionPass();
    }
}
