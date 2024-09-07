using Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

[ComplexType]
public class Location : IEquatable<Location>
{
    public int Timeline { get; set; }
    public int Year { get; set; }
    public Phase Phase { get; set; }

    [MaxLength(5)]
    public string RegionId { get; set; } = null!;

    public bool Equals(Location? other)
        => Timeline == other?.Timeline && Year == other?.Year && Phase == other?.Phase && RegionId == other?.RegionId;

    public static bool operator ==(Location? left, Location? right) => left?.Equals(right) ?? right == null;

    public static bool operator !=(Location? left, Location? right) => !(left == right);

    public override bool Equals(object? obj) => Equals(obj as Location);

    public override int GetHashCode() => (Timeline, Year, Phase, RegionId).GetHashCode();

    [NotMapped]
    public List<Move> AttackingMoves { get; set; } = new List<Move>();

    [NotMapped]
    public OrderStrength HoldStrength { get; set; } = new();

    [NotMapped]
    public virtual Order? OrderAtLocation { get; set; }
}
