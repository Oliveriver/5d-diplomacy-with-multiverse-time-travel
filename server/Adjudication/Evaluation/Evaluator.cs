using Entities;
using Enums;

namespace Adjudication;

public class Evaluator
{
    private readonly World world;
    private readonly List<Order> activeOrders;

#pragma warning disable IDE0052 // Remove unread private members
    private readonly AdjacencyValidator adjacencyValidator;
#pragma warning restore IDE0052 // Remove unread private members
    private readonly AdjustmentEvaluator adjustmentEvaluator;

    public Evaluator(World world, AdjacencyValidator adjacencyValidator)
    {
        this.world = world;
        this.adjacencyValidator = adjacencyValidator;

        activeOrders = GetActiveOrders();
        adjustmentEvaluator = new(world, activeOrders);
    }

    public void EvaluateOrders()
    {
        adjustmentEvaluator.EvaluateAdjustments();

        // REPLACE
        foreach (var order in activeOrders)
        {
            if (order.Status == OrderStatus.New)
            {
                order.Status = OrderStatus.Success;
            }

            var touchedBoards = world.Boards.Where(b => order.TouchedLocations.Any(l => b.Contains(l))).ToList();

            foreach (var board in touchedBoards)
            {
                board.MightAdvance = true;
            }
        }

        // TODO
        // - Implement adjudication algorithm
        // - Mark orders as success/failure
        // - Mark units needing retreat or add disbands if not possible (use adjacencyValidator, maybe add new retreat states to OrderStatus)
    }

    private List<Order> GetActiveOrders()
    {
        var newOrders = world.Orders.Where(o => o.Status == OrderStatus.New).ToList();

        var idleUnits = world.ActiveBoards
            .Where(b => b.Phase != Phase.Winter)
            .SelectMany(b => b.Units)
            .Where(u => newOrders.All(o => o.Unit != u));

        foreach (var unit in idleUnits)
        {
            var hold = new Hold
            {
                Status = OrderStatus.New,
                Unit = unit,
                UnitId = unit.Id,
                Location = unit.Location,
            };
            world.Orders.Add(hold);
            newOrders.Add(hold);
        }

        var depthFirstSearch = new DepthFirstSearch(world, newOrders);
        var touchedOrders = newOrders.SelectMany(depthFirstSearch.GetTouchedOrders);

        var activeOrders = newOrders.Concat(touchedOrders).ToList();
        foreach (var order in activeOrders)
        {
            var touchedBoards = world.Boards.Where(b => order.TouchedLocations.Any(l => b.Contains(l))).ToList();
            foreach (var board in touchedBoards)
            {
                board.MightAdvance = true;
            }
        }

        return activeOrders;
    }

    private class DepthFirstSearch(World world, List<Order> newOrders)
    {
        private readonly World world = world;
        private readonly List<Order> touchedOrders = [.. newOrders];

        public List<Order> GetTouchedOrders(Order order)
        {
            AddTouchedOrders(order);
            return touchedOrders;
        }

        private void AddTouchedOrders(Order order)
        {
            if (!touchedOrders.Contains(order))
            {
                touchedOrders.Add(order);
            }

            var adjacentOrders = world.Orders.Where(o => o != order && o.TouchedLocations.Intersect(order.TouchedLocations).Any());
            var newAdjacentOrders = adjacentOrders.Where(o => !touchedOrders.Contains(o));

            foreach (var newOrder in newAdjacentOrders)
            {
                AddTouchedOrders(newOrder);
            }
        }
    }
}
