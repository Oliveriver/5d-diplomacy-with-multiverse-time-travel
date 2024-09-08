namespace Entities;

public class Build : Order
{
    public override string ToString() => $"Build {Location}: {Status}";
}
