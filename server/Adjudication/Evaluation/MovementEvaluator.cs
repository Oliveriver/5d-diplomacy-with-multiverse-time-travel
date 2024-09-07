using Entities;
using Enums;

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

    public void InitialiseAdjudication()
    {
        foreach (var order in activeOrders)
        {
            order.Location.OrderAtLocation = order;
        }

        foreach (var move in activeOrders.OfType<Move>())
        {
            move.Destination.AttackingMoves.Add(move);
        }

        foreach (var support in activeOrders.OfType<Support>())
        {
            //add each support to the PotentialSupports list in the corresponding order
            support.Midpoint.OrderAtLocation!.PotentialSupports.Add(support);

            AdjudicateSupport(support);
        }
    }

    //public void AdjudicateMoveViaConvoy(Move move)
    //{
    //TODO
    //}

    public void AdjudicateConvoy(Convoy convoy)
    {
        var unresolved = false;
        foreach (var attackingMove in convoy.Location.AttackingMoves)
        {
            if (attackingMove.Status == OrderStatus.Success)
            {
                convoy.Status = OrderStatus.Failure;
            }
            else if (attackingMove.Status != OrderStatus.Failure)
            {
                unresolved = true;
            }
        }

        if (!unresolved)
        {
            convoy.Status = OrderStatus.Success;
        }
    }

    public void AdjudicateSupport(Support support)
    {
        var unresolved = false;
        foreach (var attackingMove in support.Location.AttackingMoves)
        {
            if (!Object.ReferenceEquals(support.Midpoint,support.Destination) && !Object.ReferenceEquals(support.Destination, attackingMove.Location))
            {
                support.Status = OrderStatus.Failure;
            }
            else
            {
                unresolved = true;
            }
            //TODO - if AttackingMove is Move via Convoy and AttackingMove is unresolved, then unresolved = true
        }

        if (!unresolved)
        {
            support.Status = OrderStatus.Success;
        }
    }

    public void AdjudicateMove(Move move)
    {

        var maxPreventStr = 0;
        var minPreventStr = 0;
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

        if (move.OpposingMove != null && move.AttackStrength.Min > move.OpposingMove.DefendStrength.Max || move.OpposingMove == null && move.AttackStrength.Min > move.Destination.HoldStrength.Max)
        {
            if (move.AttackStrength.Min > maxPreventStr)
            {
                move.Status = OrderStatus.Success;
                if (move.OpposingMove != null)
                {
                    move.OpposingMove.Unit!.MustRetreat = true;
                }
                else if (move.Destination.OrderAtLocation != null && move.Destination.OrderAtLocation is not Move)
                {
                    move.Destination.OrderAtLocation.Status = OrderStatus.Failure;
                    move.Destination.OrderAtLocation.Unit!.MustRetreat = true;
                    //This also needs to be done if the OrderAtDestination is an unsuccessful move, but that might not be determined yet.
                    //So I'm thinking to maybe remove the MustRetreat line from here from here and calculate which units are dislodged after everything else is done.
                }

                foreach (var attackingMove in move.Destination.AttackingMoves)
                {
                    if (!Object.ReferenceEquals(attackingMove, move))
                    {
                        attackingMove.Status = OrderStatus.Failure;
                    }
                }
            }
        }

        if (move.OpposingMove != null && move.AttackStrength.Max <= move.OpposingMove.DefendStrength.Min || move.OpposingMove == null && move.AttackStrength.Max <= move.Destination.HoldStrength.Min || move.AttackStrength.Max <= minPreventStr)
        {
            //unsuccessful
            move.Status = OrderStatus.Failure;
        }
    }

    public void OrderStrengthCalculator(Order order)
    {
        if (order is Move move)
        {
            MoveStrengthCalculator(move);
        }
        else
        {
            //for all other order types, we only need Hold Strength
            //initialise to base unit strength of 1
            order.HoldStrength.Min = 1;
            order.HoldStrength.Max = 1;
            foreach (var support in order.PotentialSupports)
            {
                //all successful supports add 1 to min and max, all unresolved add 1 to max.
                if (support.Status == OrderStatus.Success)
                {
                    order.HoldStrength.Max += 1;
                    order.HoldStrength.Min += 1;
                }
                else if (support.Status != OrderStatus.Failure)
                {
                    order.HoldStrength.Max += 1;
                }
            }
            //copy over HoldStrength to the Location, as this makes adjudication more simple
            order.Location.HoldStrength.Min = order.HoldStrength.Min;
            order.Location.HoldStrength.Max = order.HoldStrength.Max;
        }
    }

    public void MoveStrengthCalculator(Move move)
    {
        //initialise all strengths to the base strength of the unit
        move.AttackStrength.Min = 1;
        move.AttackStrength.Max = 1;
        move.HoldStrength.Min = 1;
        move.HoldStrength.Max = 1;
        move.DefendStrength.Min = 1;
        move.DefendStrength.Max = 1;
        move.PreventStrength.Min = 1;
        move.PreventStrength.Max = 1;
        //Hold Strength
        if (move.Status == OrderStatus.Success)
        {
            //if a move is successful, its hold strength is 0
            move.HoldStrength.Min = 0;
            move.HoldStrength.Max = 0;
        }
        else if (move.Status == OrderStatus.Failure)
        {
            //if a move fails, its hold strength is 1
            move.HoldStrength.Min = 1;
            move.HoldStrength.Max = 1;
        }
        else
        {
            //if unknown resolution, could be 0 or 1
            move.HoldStrength.Min = 0;
            move.HoldStrength.Max = 1;
        }
        //copy over HoldStrength to the Location, as this makes adjudication more simple
        move.Location.HoldStrength.Min = move.HoldStrength.Min;
        move.Location.HoldStrength.Max = move.HoldStrength.Max;

        //Attack Strength
        //TODO - this still needs convoy logic. If the move is via convoy the min strength is always 0 until a Successful path is determined.
        var minAttackStrengthSet = false;
        var maxAttackStrengthSet = false;
        if (move.Destination.OrderAtLocation != null && move.Unit!.Owner == move.Destination.OrderAtLocation.Unit!.Owner)
        {
            if (move.Destination.OrderAtLocation is Move)
            {
                if (move.Destination.OrderAtLocation.Status == OrderStatus.Failure)
                {
                    //if the unit at the destination belongs to the same country and is moving, the attack strength is 0 if that unit fails to move.
                    move.AttackStrength.Min = 0;
                    move.AttackStrength.Max = 0;
                    minAttackStrengthSet = true;
                    maxAttackStrengthSet = true;
                }
                else if (move.Destination.OrderAtLocation.Status != OrderStatus.Success)
                {
                    //if that destination move is unresolved, then the min attack strength is the case where it fails.
                    move.AttackStrength.Min = 0;
                    minAttackStrengthSet = true;
                }
            }
            else
            {
                //if the unit at the destination belongs to the same country and is not moving, the attack strength is always 0.
                move.AttackStrength.Min = 0;
                move.AttackStrength.Max = 0;
                minAttackStrengthSet = true;
                maxAttackStrengthSet = true;
            }
        }

        if (!maxAttackStrengthSet)
        {
            foreach (var support in move.PotentialSupports)
            {
                if (move.Destination.OrderAtLocation == null || support.Unit!.Owner != move.Destination.OrderAtLocation.Unit!.Owner || move.Destination.OrderAtLocation.Status == OrderStatus.Success)
                {
                    //a support will not add to the attack strength if it belongs to the same country as the unit at the destination, unless that destination move was successful
                    //otherwise all successful supports add to the min and max str, and all unresolved supports add to the max str (where min and max have not been set to 0 previously).
                    if (support.Status == OrderStatus.Success)
                    {
                        move.AttackStrength.Max += 1;
                        if (!minAttackStrengthSet)
                        {
                            move.AttackStrength.Min += 1;
                        }
                    }
                    else if (support.Status != OrderStatus.Failure)
                    {
                        move.AttackStrength.Max += 1;
                    }
                }
                else if (move.Destination.OrderAtLocation.Status != OrderStatus.Failure)
                {
                    //if that destination move is unresolved, then a support belonging to the same country can be counted, but only for max strength.
                    if (support.Status != OrderStatus.Failure)
                    {
                        move.AttackStrength.Max += 1;
                    }
                }
            }
        }
        //Defend Strength
        foreach (var support in move.PotentialSupports)
        {
            //all successful supports add 1 to min and max, all unresolved add 1 to max.
            if (support.Status == OrderStatus.Success)
            {
                move.DefendStrength.Max += 1;
                move.DefendStrength.Min += 1;
            }
            else if (support.Status != OrderStatus.Failure)
            {
                move.DefendStrength.Max += 1;
            }
        }

        //Prevent Strength
        //TODO - this also still needs convoy logic. Will be same as Attack Strength.
        var minPreventStrengthSet = false;
        var maxPreventStrengthSet = false;
        if (move.OpposingMove != null)
        {
            if (move.OpposingMove.Status == OrderStatus.Success)
            {
                //if the head to head attacker is successful, a move order has no attack strength
                move.PreventStrength.Min = 0;
                move.PreventStrength.Max = 0;
                minPreventStrengthSet = true;
                maxPreventStrengthSet = true;
            }
            else if (move.OpposingMove.Status != OrderStatus.Failure)
            {
                //head to head attacker has not yet been resolved, so minimum value is the case where the head to head attacker is successful
                move.PreventStrength.Min = 0;
                minPreventStrengthSet = true;
            }
        }

        if (!maxPreventStrengthSet)
        {
            foreach (var support in move.PotentialSupports)
            {
                //Min Prevent Str is 1 + number of successful supports, if not already set to 0.
                //Max Prevent Str is 1 + number of successful or unresolved supports, if not already set to 0.
                if (support.Status == OrderStatus.Success)
                {
                    move.AttackStrength.Max += 1;
                    if (!minPreventStrengthSet)
                    {
                        move.PreventStrength.Min += 1;
                    }
                }
                else if (support.Status != OrderStatus.Failure)
                {
                    move.PreventStrength.Max += 1;
                }
            }
        }
    }
}

