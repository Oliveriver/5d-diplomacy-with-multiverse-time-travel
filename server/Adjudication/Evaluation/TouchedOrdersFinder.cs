using Entities;
using Enums;

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

        var retreats = depthFirstSearch.TouchedOrders.Where(o =>
            o.Status is OrderStatus.RetreatNew
            or OrderStatus.RetreatSuccess
            or OrderStatus.RetreatFailure
            or OrderStatus.RetreatInvalid).ToList();

        return hasRetreats ? retreats : [.. depthFirstSearch.TouchedOrders.Except(retreats)];
    }

    private class DepthFirstSearch(World world, List<Order> newOrders, AdjacencyValidator adjacencyValidator)
    {
        private readonly World world = world;

        private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;

        public List<Order> TouchedOrders { get; } = [.. newOrders];

        public void AddTouchedOrders(Order order)
        {
            if (!TouchedOrders.Contains(order))
            {
                TouchedOrders.Add(order);
            }

            var adjacentOrders = world.Orders.Where(o => o != order && AreTouching(order, o));
            var newAdjacentOrders = adjacentOrders.Where(o => !TouchedOrders.Contains(o));

            foreach (var newOrder in newAdjacentOrders)
            {
                AddTouchedOrders(newOrder);
            }
        }

        private bool AreTouching(Order order1, Order order2)
        {
            foreach (var location1 in order1.TouchedLocations())
            {
                foreach (var location2 in order2.TouchedLocations())
                {
                    if (adjacencyValidator.EqualsOrIsRelated(location1, location2))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
