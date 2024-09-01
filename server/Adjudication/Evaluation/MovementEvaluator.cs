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
    }

    public void AdjudicateSupport(Support support)
    {
        foreach (var attackingMove in support.Location.AttackingMoves)
        {
            if (!Object.ReferenceEquals(support.Midpoint,support.Destination) && !Object.ReferenceEquals(support.Destination, attackingMove.Location))
            {
                //TODO - also need to check if it's an unresolved convoy move first...
                support.Status = Enums.OrderStatus.Failure;
            }
        }

    }

    public void AdjudicateMove(Move move)
    {

        int maxPreventStr = 0;
        int minPreventStr = 0;
        foreach (var attackingMove in move.Destination.AttackingMoves)
        {
            if (!Object.ReferenceEquals(attackingMove, move))
            {
                if (attackingMove.PreventStrength.Max > maxPreventStr)
                {
                    maxPreventStr = attackingMove.PreventStrength.Max;
                }
                if (attackingMove.PreventStrength.Min > minPreventStr)
                {
                    minPreventStr = attackingMove.PreventStrength.Min;
                }
            }
        }

        if (((move.OpposingMove != null) && (move.AttackStrength.Min > move.OpposingMove.DefendStrength.Max)) || ((move.OpposingMove == null) && (move.AttackStrength.Min > move.Destination.HoldStrength.Max)))
        {
            if (move.AttackStrength.Min > maxPreventStr)
            {
                move.Status = Enums.OrderStatus.Success;
                if(move.OpposingMove != null)
                {
                    move.OpposingMove.Unit.MustRetreat = true;
                }
                //Need to also check if the opposing territory has a unit present and dislodge it if true

                foreach(var attackingMove in move.Destination.AttackingMoves)
                {
                    if(!Object.ReferenceEquals(attackingMove, move))
                    {
                        attackingMove.Status = Enums.OrderStatus.Failure;
                    }
                }
            }
        }

        if (((move.OpposingMove != null) && (move.AttackStrength.Max <= move.OpposingMove.DefendStrength.Min)) || ((move.OpposingMove == null) && (move.AttackStrength.Max <= move.Destination.HoldStrength.Min)) || (move.AttackStrength.Max <= minPreventStr))
        {
            //unsuccessful
        }
    }
}

