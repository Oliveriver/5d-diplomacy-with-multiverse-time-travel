using Entities;
using Enums;

namespace Adjudication;

public class OrderSetResolver(World world, List<Order> orders, List<Region> regions, AdjacencyValidator adjacencyValidator)
{
    private readonly World world = world;

    private readonly List<Region> regions = regions;

    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;
    private readonly OrderResolver orderResolver = new(orders, adjacencyValidator);
    private readonly DependencyCalculator dependencyCalculator = new(orders, adjacencyValidator);
    private readonly StrengthCalculator strengthCalculator = new(orders, adjacencyValidator);

    private readonly List<Move> moves = orders.OfType<Move>().Where(m => m.Status != OrderStatus.Invalid).ToList();
    private readonly List<Support> supports = orders.OfType<Support>().Where(s => s.Status != OrderStatus.Invalid).ToList();
    private readonly List<Convoy> convoys = orders.OfType<Convoy>().Where(c => c.Status != OrderStatus.Invalid).ToList();

    public void RunResolutionAlgorithm()
    {
        ApplyInitialPass();

        var unresolvedOrders = orders.Where(o => o.Status == OrderStatus.New).ToList();

        while (unresolvedOrders.Count > 0)
        {
            foreach (var order in unresolvedOrders)
            {
                Resolve(order, []);
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

    private void ApplyInitialPass()
    {
        var initialStatuses = orders.Select(o => o.Status).ToList();
        var newStatuses = new List<OrderStatus>();

        UpdateConvoyPaths();
        UpdateOrderStrengths();

        while (!initialStatuses.SequenceEqual(newStatuses))
        {
            initialStatuses = newStatuses;

            foreach (var order in orders)
            {
                orderResolver.TryResolve(order);
            }

            UpdateConvoyPaths();
            UpdateOrderStrengths();
            UpdateSelfAttackingSupports();

            newStatuses = orders.Select(o => o.Status).ToList();
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

            if (destinationOrder == null || destinationOrder.Unit!.Owner != support.Unit!.Owner)
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

    private void Resolve(Order order, List<Order> currentStack)
    {
        if (currentStack.Contains(order))
        {
            ApplyBackupRule(currentStack);
            return;
        }

        UpdateConvoyPaths();
        UpdateOrderStrengths();

        orderResolver.TryResolve(order);

        if (order.Status != OrderStatus.New)
        {
            return;
        }

        var dependents = dependencyCalculator.GetDependents(order);

        currentStack.Add(order);

        foreach (var dependent in dependents)
        {
            Resolve(dependent, currentStack);
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
            Resolve(guessedOrder, []);
        }
        else if (isConsistentFailure && !isConsistentSuccess)
        {
            guessedOrder.Status = OrderStatus.Failure;
            Resolve(guessedOrder, []);
        }
        else if (!isConsistentSuccess && !isConsistentFailure)
        {
            ApplySzykmanRule(guessedOrder, currentStack);
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
                Resolve(guessedOrder, []);
            }
            else
            {
                ApplySzykmanRule(guessedOrder, currentStack);
            }
        }
    }

    private bool TryGuessStatus(Order order, OrderStatus status)
    {
        var initialStatuses = orders.Select(o => o.Status).ToList();

        order.Status = status;
        Resolve(order, []);

        var isConsistent = order.Status == status;

        foreach (var modifiedOrder in orders)
        {
            modifiedOrder.Status = initialStatuses[orders.IndexOf(modifiedOrder)];
        }

        return isConsistent;
    }

    private void ApplySzykmanRule(Order order, List<Order> currentStack)
    {
        var movesViaConvoy = currentStack.OfType<Move>().Where(m => m.ConvoyPath.Count > 0);
        foreach (var move in movesViaConvoy)
        {
            move.Status = OrderStatus.Failure;
            move.IsSzykmanHold = true;
        }

        Resolve(order, []);
    }
}
