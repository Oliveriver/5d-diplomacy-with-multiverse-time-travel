namespace Entities;

public class Disband : Order
{
    public override string ToString() => $"Disband {Location}: {Status}";
}
