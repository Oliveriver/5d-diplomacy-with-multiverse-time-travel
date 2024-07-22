using Enums;

namespace Models;

public class World(int iteration, Board[] boards, Order[] orders, Nation? winner)
{
    public int Iteration { get; set; } = iteration;
    public Board[] Boards { get; set; } = boards;
    public Order[] Orders { get; set; } = orders;
    public Nation? Winner { get; set; } = winner;
}
