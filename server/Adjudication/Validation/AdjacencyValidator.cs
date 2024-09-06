using Entities;
using Enums;

namespace Adjudication;

public class AdjacencyValidator(List<Region> regions, bool hasStrictAdjacencies)
{
    private readonly List<Region> regions = regions;
    private readonly bool hasStrictAdjacencies = hasStrictAdjacencies;

    public bool IsValidDirectMove(
        Unit unit,
        Location location,
        Location destination,
        bool allowDestinationSibling = false,
        bool allowDestinationChild = false)
    {
        if (location.Phase == Phase.Winter || destination.Phase == Phase.Winter)
        {
            return false;
        }

        var isSameBoard = location.Timeline == destination.Timeline
            && location.Year == destination.Year
            && location.Phase == destination.Phase;

        return isSameBoard
            ? IsValidIntraBoardMove(unit, location, destination, allowDestinationSibling, allowDestinationChild)
            : !unit.MustRetreat && IsValidInterBoardMove(unit, location, destination, allowDestinationSibling, allowDestinationChild);
    }

    public bool IsValidInterBoardMove(Unit unit, Location location, Location destination, bool allowDestinationSibling, bool allowDestinationChild)
    {
        var locationId = location.RegionId;
        var destinationId = destination.RegionId;

        if (hasStrictAdjacencies && locationId != destinationId)
        {
            return false;
        }
        else if (!IsValidIntraBoardMove(unit, location, destination, allowDestinationSibling, allowDestinationChild))
        {
            return false;
        }

        var yearDistance = location.Year - destination.Year;
        var phaseDistance = (int)location.Phase - (int)destination.Phase;
        var timeDistance = Math.Abs(2 * yearDistance + phaseDistance);

        var multiverseDistance = Math.Abs(location.Timeline - destination.Timeline);

        return timeDistance <= 1 && multiverseDistance <= 1 && (timeDistance == 0 || multiverseDistance == 0);
    }

    public bool IsValidIntraBoardMove(Unit unit, Location location, Location destination, bool allowDestinationSibling, bool allowDestinationChild)
    {
        if (location == destination)
        {
            return false;
        }

        if (location.RegionId == destination.RegionId)
        {
            return true;
        }

        var locationRegion = regions.First(r => r.Id == location.RegionId);
        var destinationRegion = regions.First(r => r.Id == destination.RegionId);

        if (allowDestinationSibling)
        {
            if (destinationRegion.ParentId != null)
            {
                var destinationRegionSiblings = regions.Where(r =>
                    r != destinationRegion
                    && r.ParentId == destinationRegion.ParentId);

                foreach (var siblingRegion in destinationRegionSiblings)
                {
                    var sibling = new Location
                    {
                        Timeline = destination.Timeline,
                        Year = destination.Year,
                        Phase = destination.Phase,
                        RegionId = siblingRegion.Id,
                    };

                    var isValidMove = IsValidIntraBoardMove(unit, location, sibling, false, false);
                    if (isValidMove)
                    {
                        return true;
                    }
                }
            }
        }

        var connection = locationRegion.Connections.FirstOrDefault(c => c.Regions.Any(r => r == destinationRegion));
        if (connection == null)
        {
            if (!allowDestinationChild)
            {
                return false;
            }

            var destinationRegionChildren = regions.Where(r => r.ParentId == destination.RegionId);
            connection = locationRegion.Connections.FirstOrDefault(c => c.Regions.Intersect(destinationRegionChildren).Any());
            if (connection == null)
            {
                return false;
            }
        }

        var isValidArmyMove = unit.Type == UnitType.Army && connection.Type != ConnectionType.Sea;
        var isValidFleetMove = unit.Type == UnitType.Fleet && connection.Type != ConnectionType.Land;
        return isValidArmyMove || isValidFleetMove;
    }

    public bool EqualsOrIsRelated(Location location1, Location location2)
    {
        var location1Region = regions.First(r => r.Id == location1.RegionId);
        var location2Region = regions.First(r => r.Id == location2.RegionId);

        var location1ParentRegion = regions.FirstOrDefault(r => r.Id == location1Region.ParentId);
        var location2ParentRegion = regions.FirstOrDefault(r => r.Id == location1Region.ParentId);

        if (location1ParentRegion == null)
        {
            if (location2ParentRegion == null)
            {
                return location1 == location2;
            }

            var location2Parent = new Location
            {
                Timeline = location2.Timeline,
                Year = location2.Year,
                Phase = location2.Phase,
                RegionId = location2ParentRegion.Id,
            };

            return location1 == location2Parent;
        }
        else if (location2ParentRegion == null)
        {
            var location1Parent = new Location
            {
                Timeline = location2.Timeline,
                Year = location2.Year,
                Phase = location2.Phase,
                RegionId = location1ParentRegion.Id,
            };

            return location2 == location1Parent;
        }
        else
        {
            var location1Parent = new Location
            {
                Timeline = location1.Timeline,
                Year = location1.Year,
                Phase = location1.Phase,
                RegionId = location1ParentRegion.Id,
            };

            var location2Parent = new Location
            {
                Timeline = location2.Timeline,
                Year = location2.Year,
                Phase = location2.Phase,
                RegionId = location2ParentRegion.Id,
            };

            return location1Parent == location2Parent;
        }
    }
}

