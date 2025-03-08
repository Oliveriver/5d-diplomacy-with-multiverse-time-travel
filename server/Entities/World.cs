using Enums;
using System.ComponentModel.DataAnnotations.Schema;
using Utilities;

namespace Entities;

public class World
{
    public int Id { get; set; }

    public int GameId { get; set; }
    public virtual Game Game { get; set; } = null!;

    public virtual List<Board> Boards { get; set; } = [];
    public virtual List<Order> Orders { get; set; } = [];
    public int Iteration { get; set; }
    public Nation? Winner { get; set; }

    [NotMapped]
    public List<Board> ActiveBoards
        => [.. Boards.GroupBy(b => b.Timeline).Select(t => t.MaxBy(b => 3 * b.Year + (int)b.Phase)).OfType<Board>()];

    [NotMapped]
    public List<Nation> LivingPlayers
        => [.. Constants.Nations.Where(n => ActiveBoards.Any(b => b.Centres.Any(c => c.Owner == n)))];

    public bool HasRetreats() => Boards.SelectMany(b => b.Units).Any(u => u.MustRetreat);
}
