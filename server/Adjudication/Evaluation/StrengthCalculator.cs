using Entities;
using Enums;

namespace Adjudication;

public class StrengthCalculator(List<Order> activeOrders, AdjacencyValidator adjacencyValidator)
{
    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;

    private readonly List<Order> activeOrders = activeOrders;

    public void UpdateOrderStrength(Order order)
    {
        if (order is Move move)
        {
            CalculateMoveStrength(move);
            return;
        }

        order.HoldStrength.Max = 1;
        order.HoldStrength.Min = 1;

        foreach (var support in order.Supports)
        {
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
    }

    private void CalculateMoveStrength(Move move)
    {
        CalculateMoveHoldStrength(move);
        CalculateMoveDefendStrength(move);
        CalculateMoveAttackStrength(move);
        CalculateMovePreventStrength(move);
    }

    private void CalculateMoveHoldStrength(Move move)
    {
        switch (move.Status)
        {
            case OrderStatus.Success:
            case OrderStatus.Retreat:
                {
                    move.HoldStrength.Max = 0;
                    move.HoldStrength.Min = 0;
                    break;
                }
            case OrderStatus.Invalid:
            case OrderStatus.Failure:
                {
                    move.HoldStrength.Max = 1;
                    move.HoldStrength.Min = 1;
                    break;
                }
            case OrderStatus.New:
            case OrderStatus.Guessing:
            default:
                {
                    move.HoldStrength.Max = 1;
                    move.HoldStrength.Min = 0;
                    break;
                }
        }
    }

    private void CalculateMoveDefendStrength(Move move)
    {
        move.DefendStrength.Max = 1;
        move.DefendStrength.Min = 1;

        AddSupportStrength(move.DefendStrength, move.Supports);
    }

    private void CalculateMoveAttackStrength(Move move)
    {
        move.AttackStrength.Max = 1;
        move.AttackStrength.Min = 1;

        if (move.ConvoyPath.Any(c => c.Status == OrderStatus.New))
        {
            move.AttackStrength.Min = 0;

            if (move.ConvoyPath.Any(c => c.Status == OrderStatus.Failure))
            {
                move.AttackStrength.Max = 0;
            }

            return;
        }

        var destinationOrder = activeOrders.FirstOrDefault(o => adjacencyValidator.EqualsOrIsRelated(o.Location, move.Destination));

        if (destinationOrder == null ||
            destinationOrder is Move destinationMove && destinationMove.OpposingMove == null && destinationMove.Status == OrderStatus.Success)
        {
            AddSupportStrength(move.AttackStrength, move.Supports);
            return;
        }

        if (destinationOrder.Unit!.Owner == move.Unit!.Owner)
        {
            move.AttackStrength.Max = 0;
            move.AttackStrength.Min = 0;
            return;
        }

        AddSupportStrength(move.AttackStrength, move.Supports.Where(s => s.Unit!.Owner != destinationOrder.Unit!.Owner).ToList());
    }

    private void CalculateMovePreventStrength(Move move)
    {
        move.PreventStrength.Max = 1;
        move.PreventStrength.Min = 1;

        if (move.ConvoyPath.Any(c => c.Status == OrderStatus.New))
        {
            move.PreventStrength.Min = 0;

            if (move.ConvoyPath.Any(c => c.Status == OrderStatus.Failure))
            {
                move.PreventStrength.Max = 0;
            }

            return;
        }

        if (move.OpposingMove?.Status == OrderStatus.Success)
        {
            move.PreventStrength.Max = 0;
            move.PreventStrength.Min = 0;
            return;
        }

        AddSupportStrength(move.PreventStrength, move.Supports);
    }

    private void AddSupportStrength(OrderStrength strength, List<Support> supports)
    {
        foreach (var support in supports)
        {
            switch (support.Status)
            {
                case OrderStatus.Success:
                    {
                        strength.Max += 1;
                        strength.Min += 1;
                        break;
                    }
                case OrderStatus.New:
                case OrderStatus.Guessing:
                    {
                        strength.Max += 1;
                        break;
                    }
                case OrderStatus.Invalid:
                case OrderStatus.Failure:
                case OrderStatus.Retreat:
                default:
                    break;
            }
        }
    }
}
