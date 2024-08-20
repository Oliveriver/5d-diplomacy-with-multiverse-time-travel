using Entities;

namespace Adjudication;

public class MapComparer : EqualityComparer<Board>
{
    private readonly CentreComparer centreComparer = new();
    private readonly UnitComparer unitComparer = new();

    public override bool Equals(Board? x, Board? y)
    {
        if (x == null || y == null)
        {
            return x == y;
        }

        var hasMatchingCentres = x.Centres
            .OrderBy(c => c.Location.RegionId)
            .SequenceEqual(y.Centres.OrderBy(c => c.Location.RegionId),
            centreComparer);

        if (!hasMatchingCentres)
        {
            return false;
        }

        var hasMatchingUnits = x.Units
            .OrderBy(u => u.Location.RegionId)
            .SequenceEqual(y.Units.OrderBy(u => u.Location.RegionId),
            unitComparer);

        return hasMatchingUnits;
    }

    public override int GetHashCode(Board obj)
    {
        var centreHashes = obj.Centres.Select(centreComparer.GetHashCode);
        var unitHashes = obj.Units.Select(unitComparer.GetHashCode);

        return (centreHashes, unitHashes).GetHashCode();
    }

    private class CentreComparer : EqualityComparer<Centre>
    {
        public override bool Equals(Centre? centre1, Centre? centre2)
            => centre1?.Owner == centre2?.Owner && centre1?.Location.RegionId == centre2?.Location.RegionId;
        public override int GetHashCode(Centre centre) => (centre.Location.RegionId, centre.Owner).GetHashCode();
    }

    private class UnitComparer : EqualityComparer<Unit>
    {
        public override bool Equals(Unit? unit1, Unit? unit2)
            => unit1?.Owner == unit2?.Owner
            && unit1?.Location.RegionId == unit2?.Location.RegionId
            && unit1?.Type == unit2?.Type
            && unit1?.MustRetreat == unit2?.MustRetreat;

        public override int GetHashCode(Unit unit) => (unit.Type, unit.Location.RegionId, unit.Owner, unit.MustRetreat).GetHashCode();
    }
}
