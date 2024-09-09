namespace Entities;

public class Hold : Order
{
    public override string ToString() => $"Hold {Location}: {Status}";
}
