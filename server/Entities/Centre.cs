using Enums;

namespace Entities;

public class Centre
{
    public int Id { get; set; }

    public int BoardId { get; set; }
    public virtual Board Board { get; set; } = null!;

    public Location Location { get; set; } = null!;
    public Nation? Owner { get; set; }
}
