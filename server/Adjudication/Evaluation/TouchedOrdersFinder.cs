using Entities;

namespace Adjudication;

public class TouchedOrdersFinder(World world, AdjacencyValidator adjacencyValidator)
{
    private readonly World world = world;

    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;

    public List<Order> GetTouchedOrders(List<Order> orders, bool hasRetreats)
    {
        var depthFirstSearch = new DepthFirstSearch(world, orders, adjacencyValidator);
        foreach (var order in orders)
        {
            depthFirstSearch.AddTouchedOrders(order);
        }

        return [..
            hasRetreats
                ? depthFirstSearch.TouchedOrders.Where(o => o.IsRetreat())
                : depthFirstSearch.TouchedOrders.Where(o => !o.IsRetreat())
            ];
    }

    private class DepthFirstSearch(World world, List<Order> newOrders, AdjacencyValidator adjacencyValidator)
    {
        private readonly ILookup<Location, Order> worldOrdersByTouchedLocation = world.Orders
            .SelectMany(o => o.TouchedLocations().Select(l => (Order: o, TouchedLocation: adjacencyValidator.ParentLocation(l))))
            .ToLookup(x => x.TouchedLocation, x => x.Order);

        private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;

        public HashSet<Order> TouchedOrders { get; } = new HashSet<Order>(newOrders.Count);

        public void AddTouchedOrders(Order order)
        {
            if (!TouchedOrders.Add(order))
            {
                return;
            }

            var touchedLocations = order.TouchedLocations().Select(adjacencyValidator.ParentLocation);
            var newAdjacentOrders = touchedLocations.SelectMany(l => worldOrdersByTouchedLocation[l]);

            foreach (var newOrder in newAdjacentOrders)
            {
                AddTouchedOrders(newOrder);
            }
        }
    }
}
