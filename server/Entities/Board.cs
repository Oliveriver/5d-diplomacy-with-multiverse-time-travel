using Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public class Board
{
    public int Id { get; set; }

    public int WorldId { get; set; }
    public virtual World World { get; set; } = null!;

    public int Timeline { get; set; }
    public int Year { get; set; }
    public Phase Phase { get; set; }
    public List<int> ChildTimelines { get; set; } = [];
    public virtual List<Centre> Centres { get; set; } = [];
    public virtual List<Unit> Units { get; set; } = [];

    [NotMapped]
    public bool MightAdvance { get; set; }

    public bool Contains(Location location)
        => Timeline == location.Timeline && Year == location.Year && Phase == location.Phase;
}
