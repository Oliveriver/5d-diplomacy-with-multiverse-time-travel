using Entities;
using Enums;

namespace Adjudication;

public class AdjacencyValidator(RegionMap regionMap, bool hasStrictAdjacencies)
{
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

        var locationRegion = regionMap.GetRegion(location.RegionId);
        var destinationRegion = regionMap.GetRegion(destination.RegionId);

        if (allowDestinationSibling)
        {
            var destinationRegionSiblings = regionMap.GetChildRegions(destinationRegion.Id);
            if (destinationRegion.Parent != null)
            {
                destinationRegionSiblings = destinationRegionSiblings
                    .Concat(regionMap.GetChildRegions(destinationRegion.Parent.Id))
                    .Append(destinationRegion.Parent);
            }

            foreach (var siblingRegion in destinationRegionSiblings)
            {
                if (siblingRegion == destinationRegion)
                {
                    continue;
                }

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

        var connection = regionMap.GetConnections(locationRegion).FirstOrDefault(c => c.Destination == destinationRegion);
        if (connection == null)
        {
            if (!allowDestinationChild)
            {
                return false;
            }

            var destinationRegionChildren = regionMap.GetChildRegions(destination.RegionId).ToHashSet();
            if (destinationRegionChildren.Count == 0)
            {
                return false;
            }

            connection = regionMap.GetConnections(locationRegion).FirstOrDefault(c => destinationRegionChildren.Contains(c.Destination));
            if (connection == null)
            {
                return false;
            }
        }

        return CanTraverseConnection(unit, connection);
    }

    public bool EqualsOrIsRelated(Location location1, Location location2)
    {
        if (location1.Timeline != location2.Timeline ||
            location1.Year != location2.Year ||
            location1.Phase != location2.Phase)
        {
            return false;
        }

        var location1Region = regionMap.GetRegion(location1.RegionId);
        var location2Region = regionMap.GetRegion(location2.RegionId);

        return (location1Region.Parent ?? location1Region) == (location2Region.Parent ?? location2Region);
    }

    public List<Location> GetAdjacentRegions(Unit unit)
    {
        var location = unit.Location;

        var locationRegion = regionMap.GetRegion(location.RegionId);
        var adjacentRegions = regionMap.GetConnections(locationRegion)
            .Where(c => CanTraverseConnection(unit, c))
            .Select(c => c.Destination);

        return [.. adjacentRegions.Select(r => new Location
        {
            Timeline = location.Timeline,
            Year = location.Year,
            Phase = location.Phase,
            RegionId = r.Id,
        })];
    }

    private static bool CanTraverseConnection(Unit unit, Connection connection)
    {
        var isValidArmyMove = unit.Type == UnitType.Army && connection.Type != ConnectionType.Sea;
        var isValidFleetMove = unit.Type == UnitType.Fleet && connection.Type != ConnectionType.Land;
        return isValidArmyMove || isValidFleetMove;
    }
}

