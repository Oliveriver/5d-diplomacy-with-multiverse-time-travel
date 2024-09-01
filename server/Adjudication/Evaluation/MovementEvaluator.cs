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

    public void AdjudicateConvoy(Convoy convoy)
    {
        bool unresolved = false;
        foreach (Move attackingMove in support.Location.AttackingMoves)
        {
            if (attackingMove.Status = Enums.OrderStatus.Success)
            {
                convoy.Status = Enums.OrderStatus.Failure;
            }
            else if (attackingMove.Status != Enums.OrderStatus.Failure)
            {
                unresolved = true;
            }
        }
        if (!unresolved)
        {
            support.Status = Enums.OrderStatus.Success;
        }
    }

    public void AdjudicateSupport(Support support)
    {
        bool unresolved = false;
        foreach (Move attackingMove in support.Location.AttackingMoves)
        {
            if (!Object.ReferenceEquals(support.Midpoint,support.Destination) && !Object.ReferenceEquals(support.Destination, attackingMove.Location))
            {
                support.Status = Enums.OrderStatus.Failure;
            }
            else
            {
                unresolved = true;
            }
            //TODO - if AttackingMove is Move via Convoy and AttackingMove is unresolved, then unresolved = true
        }
        if(!unresolved)
        {
            support.Status = Enums.OrderStatus.Success;
        }

    }

    public void AdjudicateMove(Move move)
    {

        int maxPreventStr = 0;
        int minPreventStr = 0;
        foreach (Move attackingMove in move.Destination.AttackingMoves)
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
                else if (move.Destination.OrderAtLocation != null && !(move.Destination.OrderAtLocation is Move))
                {
                    move.Destination.OrderAtLocation.Status = Enums.OrderStatus.Failure;
                    move.Destination.OrderAtLocation.Unit.MustRetreat = true;
                    //This also needs to be done if the OrderAtDestination is an unsuccessful move, but that might not be determined yet.
                    //So I'm thinking to maybe remove the MustRetreat line from here from here and calculate which units are dislodged after everything else is done.
                }

                foreach(Move attackingMove in move.Destination.AttackingMoves)
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
            move.Status = Enums.OrderStatus.Failure;
        }
    }
}

