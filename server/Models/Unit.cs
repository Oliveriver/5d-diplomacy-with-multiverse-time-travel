using Enums;

namespace Models;

public class Unit(Nation owner, UnitType type, bool mustRetreat = false)
{
    public Nation Owner { get; set; } = owner;
    public UnitType Type { get; set; } = type;
    public bool MustRetreat { get; set; } = mustRetreat;
}
