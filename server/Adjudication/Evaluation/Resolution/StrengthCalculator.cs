using Entities;
using Enums;

namespace Adjudication;

public class StrengthCalculator(List<Order> orders, AdjacencyValidator adjacencyValidator)
{
    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;

    private readonly List<Order> orders = orders;

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
                {
                    move.HoldStrength.Max = 0;
                    move.HoldStrength.Min = 0;
                    break;
                }
            case OrderStatus.Invalid:
                {
                    move.HoldStrength.Max = 1;
                    move.HoldStrength.Min = 1;

                    AddSupportStrength(move.HoldStrength, move.Supports);
                    break;
                }
            case OrderStatus.Failure:
                {
                    move.HoldStrength.Max = 1;
                    move.HoldStrength.Min = 1;
                    break;
                }
            case OrderStatus.New:
                {
                    move.HoldStrength.Max = 1;
                    move.HoldStrength.Min = 0;
                    break;
                }
            case OrderStatus.RetreatNew:
            case OrderStatus.RetreatSuccess:
            case OrderStatus.RetreatFailure:
            case OrderStatus.RetreatInvalid:
            default:
                break;
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
        if (move.Status is OrderStatus.Invalid or OrderStatus.Failure)
        {
            move.AttackStrength.Max = 0;
            move.AttackStrength.Min = 0;
            return;
        }

        move.AttackStrength.Max = 1;
        move.AttackStrength.Min = 1;

        if (move.ConvoyPath.Any(c => c.Status == OrderStatus.New))
        {
            move.AttackStrength.Min = 0;

            if (move.ConvoyPath.Any(c => c.Status == OrderStatus.Failure))
            {
                move.AttackStrength.Max = 0;
                return;
            }
        }

        var destinationOrder = orders.FirstOrDefault(o => adjacencyValidator.EqualsOrIsRelated(o.Location, move.Destination));

        if (destinationOrder != null)
        {
            if (destinationOrder.Unit.Owner == move.Unit.Owner)
            {
                if (destinationOrder is Move)
                {
                    if (destinationOrder.Status is OrderStatus.Failure or OrderStatus.Invalid)
                    {
                        move.AttackStrength.Max = 0;
                        move.AttackStrength.Min = 0;
                        return;
                    }

                    AddSupportStrength(move.AttackStrength, move.Supports);

                    if (destinationOrder.Status == OrderStatus.New)
                    {
                        move.AttackStrength.Min = 0;
                    }
                }
                else
                {
                    move.AttackStrength.Max = 0;
                    move.AttackStrength.Min = 0;
                }

                return;
            }

            if (destinationOrder.Status == OrderStatus.Success)
            {
                AddSupportStrength(move.AttackStrength, move.Supports);
                return;
            }

            var isDestinationMoveBeingConvoyed = destinationOrder is Move destinationMove && destinationMove.ConvoyPath.Any(c => c.CanProvidePath);
            if (isDestinationMoveBeingConvoyed)
            {
                AddSupportStrength(move.AttackStrength, move.Supports);
                return;
            }

            var supportsWithDifferentOwner = move.Supports.Where(s => s.Unit.Owner != destinationOrder.Unit.Owner).ToList();
            var supportsWithSameOwner = move.Supports.Where(s => s.Unit.Owner == destinationOrder.Unit.Owner).ToList();

            if (destinationOrder is not Move || destinationOrder.Status is OrderStatus.Invalid or OrderStatus.Failure)
            {
                AddSupportStrength(move.AttackStrength, supportsWithDifferentOwner);
            }
            else if (destinationOrder is Move && destinationOrder.Status == OrderStatus.Success)
            {
                AddSupportStrength(move.AttackStrength, move.Supports);
            }
            else
            {
                AddSupportStrength(move.AttackStrength, supportsWithDifferentOwner);
                AddSupportStrength(move.AttackStrength, supportsWithSameOwner, true);
            }

            return;
        }

        AddSupportStrength(move.AttackStrength, move.Supports);
    }

    private void CalculateMovePreventStrength(Move move)
    {
        if (move.Status == OrderStatus.Invalid)
        {
            move.PreventStrength.Max = 0;
            move.PreventStrength.Min = 0;
            return;
        }

        move.PreventStrength.Max = 1;
        move.PreventStrength.Min = 1;

        if (!adjacencyValidator.IsValidDirectMove(move.Unit, move.Location, move.Destination)
            && move.ConvoyPath.All(c => !c.CanProvidePath))
        {
            move.PreventStrength.Max = 0;
            move.PreventStrength.Min = 0;
            return;
        }

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

        if (move.OpposingMove?.Status == OrderStatus.New)
        {
            move.PreventStrength.Min = 0;
        }
    }

    private void AddSupportStrength(OrderStrength strength, List<Support> supports, bool isMaxOnly = false)
    {
        foreach (var support in supports)
        {
            switch (support.Status)
            {
                case OrderStatus.Success:
                    {
                        if (!isMaxOnly)
                        {
                            strength.Min += 1;
                        }

                        strength.Max += 1;
                        break;
                    }
                case OrderStatus.New:
                    {
                        strength.Max += 1;
                        break;
                    }
                case OrderStatus.Invalid:
                case OrderStatus.Failure:
                case OrderStatus.RetreatNew:
                case OrderStatus.RetreatSuccess:
                case OrderStatus.RetreatFailure:
                case OrderStatus.RetreatInvalid:
                default:
                    break;
            }
        }
    }
}
