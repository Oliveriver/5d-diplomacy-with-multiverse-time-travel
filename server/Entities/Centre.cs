using Enums;

namespace Entities;

public class Centre
{
    public int Id { get; set; }

    public int BoardId { get; set; }
    public virtual Board Board { get; set; } = null!;

    public Location Location { get; set; } = null!;
    public Nation? Owner { get; set; }

    public Centre Clone() => new()
    {
        Owner = Owner,
        Location = new()
        {
            Timeline = Location.Timeline,
            Year = Location.Year,
            Phase = Location.Phase,
            RegionId = Location.RegionId,
        }
    };
}
