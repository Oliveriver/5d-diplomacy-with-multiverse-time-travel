using Entities;
using Enums;

namespace Adjudication;

public class Evaluator
{
    private readonly World world;

    private readonly MovementEvaluator movementEvaulator;
    private readonly AdjustmentEvaluator adjustmentEvaluator;
    private readonly RetreatEvaluator retreatEvaulator;

    public Evaluator(World world, AdjacencyValidator adjacencyValidator)
    {
        this.world = world;

        var activeOrders = GetActiveOrders();

        movementEvaulator = new(world, activeOrders, adjacencyValidator);
        adjustmentEvaluator = new(world, activeOrders);
        retreatEvaulator = new(world, activeOrders, adjacencyValidator);

        foreach (var board in world.ActiveBoards)
        {
            board.MightAdvance = true;
        }
    }

    public void EvaluateOrders()
    {
        if (world.HasRetreats)
        {
            retreatEvaulator.EvaluateRetreats();
        }
        else
        {
            movementEvaulator.EvaluateMovements();
            adjustmentEvaluator.EvaluateAdjustments();
        }
    }

    private List<Order> GetActiveOrders()
    {
        var newOrders = world.Orders.Where(o => o.Status == OrderStatus.New).ToList();

        if (world.HasRetreats)
        {
            return newOrders;
        }

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
        foreach (var order in newOrders)
        {
            depthFirstSearch.AddTouchedOrders(order);
        }

        var activeOrders = depthFirstSearch.TouchedOrders;

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

        public List<Order> TouchedOrders { get; } = [.. newOrders];

        public void AddTouchedOrders(Order order)
        {
            if (!TouchedOrders.Contains(order))
            {
                TouchedOrders.Add(order);
            }

            var adjacentOrders = world.Orders.Where(o => o != order && o.TouchedLocations.Intersect(order.TouchedLocations).Any());
            var newAdjacentOrders = adjacentOrders.Where(o => !TouchedOrders.Contains(o));

            foreach (var newOrder in newAdjacentOrders)
            {
                AddTouchedOrders(newOrder);
            }
        }
    }
}
