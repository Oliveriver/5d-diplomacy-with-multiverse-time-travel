using Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public class Connection
{
    [MaxLength(11)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; } = null!;

    public ConnectionType Type { get; set; }

    public virtual List<Region> Regions { get; set; } = null!;
}

