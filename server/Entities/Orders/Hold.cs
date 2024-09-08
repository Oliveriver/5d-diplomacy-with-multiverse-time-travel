using Enums;

namespace Entities;

public class Hold : Order
{
    private OrderStatus status;

    public override OrderStatus Status
    {
        get => status;
        set
        {
            status = value;

            if (value == OrderStatus.Failure)
            {
                foreach (var support in Supports)
                {
                    support.Status = OrderStatus.Failure;
                }
            }
        }
    }

    public override string ToString() => $"Hold {Location}: {Status}";
}
