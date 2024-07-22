using Enums;

namespace Models;

public class OrderSubmissionRequest(Nation[] players, Order[] orders)
{
    public Nation[] Players { get; set; } = players;
    public Order[] Orders { get; set; } = orders;
}
