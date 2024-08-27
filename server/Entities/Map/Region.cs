using Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public class Region
{
    [MaxLength(5)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; } = null!;

    public string? ParentId { get; set; }
    public virtual Region? Parent { get; set; }

    public RegionType Type { get; set; }
    public string Name { get; set; } = null!;

    public virtual List<Connection> Connections { get; set; } = [];
    public virtual List<Region> Children { get; set; } = [];
}
