using Entities;

namespace Adjudication;

#pragma warning disable IDE0052 // Remove unread private members // TEMP
public class MovementEvaluator(World world, List<Order> activeOrders, AdjacencyValidator adjacencyValidator)
{
    private readonly World world = world;
    private readonly List<Order> activeOrders = activeOrders;

    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;

    private readonly List<Hold> holds = activeOrders.OfType<Hold>().ToList();
    private readonly List<Move> moves = activeOrders.OfType<Move>().ToList();
    private readonly List<Support> supports = activeOrders.OfType<Support>().ToList();
    private readonly List<Convoy> convoys = activeOrders.OfType<Convoy>().ToList();

    public void EvaluateMovements()
    {
        // TODO
        // - Implement adjudication algorithm
        // - Mark orders as success/failure
        // - Mark units needing retreat or add disbands if not possible (use adjacencyValidator)

        // TEMP
        foreach (var order in activeOrders.Where(o => o.Status != Enums.OrderStatus.Invalid))
        {
            order.Status = Enums.OrderStatus.Success;
        }
    }
}

