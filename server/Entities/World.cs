using Enums;

namespace Entities;

public class World
{
    public int Id { get; set; }

    public int GameId { get; set; }
    public virtual Game Game { get; set; } = null!;

    public virtual List<Board> Boards { get; set; } = null!;
    public virtual List<Order> Orders { get; set; } = null!;
    public int Iteration { get; set; }
    public Nation? Winner { get; set; }
}
