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

        static bool IsRetreat(Order o) => o.Status
            is OrderStatus.RetreatNew
            or OrderStatus.RetreatSuccess
            or OrderStatus.RetreatFailure
            or OrderStatus.RetreatInvalid;

        return [..
            hasRetreats
                ? depthFirstSearch.TouchedOrders.Where(IsRetreat)
                : depthFirstSearch.TouchedOrders.Where(o => !IsRetreat(o))
            ];
    }

    private class DepthFirstSearch(World world, List<Order> newOrders, AdjacencyValidator adjacencyValidator)
    {
        private readonly World world = world;

        private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;

        public HashSet<Order> TouchedOrders { get; } = [.. newOrders];

        public void AddTouchedOrders(Order order)
        {
            TouchedOrders.Add(order);

            var newAdjacentOrders = world.Orders.Where(o => !TouchedOrders.Contains(o) && AreTouching(order, o));

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
