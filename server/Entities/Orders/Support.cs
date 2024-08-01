using Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public class Support : Order
{
    public Location Midpoint { get; set; } = null!;
    public Location Destination { get; set; } = null!;

    [NotMapped]
    public override bool NeedsValidation => Status is OrderStatus.Invalid or OrderStatus.New;
}
