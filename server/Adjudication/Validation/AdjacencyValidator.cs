using Entities;
using Enums;

namespace Adjudication;

public class AdjacencyValidator(List<Region> regions, bool hasStrictAdjacencies)
{
    private readonly Dictionary<string, Region> regionById = regions.ToDictionary(r => r.Id);
    private readonly ILookup<string, Region> regionsByParentId = regions.Where(r => r.ParentId != null).ToLookup(r => r.ParentId!);
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

        var locationRegion = regionById[location.RegionId];
        var destinationRegion = regionById[destination.RegionId];

        if (allowDestinationSibling)
        {
            var destinationRegionSiblings = regionsByParentId[destinationRegion.Id];
            if (destinationRegion.ParentId != null)
            {
                destinationRegionSiblings = destinationRegionSiblings.Concat(regionsByParentId[destinationRegion.ParentId]);
                if (regionById.TryGetValue(destinationRegion.ParentId, out var parent))
                {
                    destinationRegionSiblings = destinationRegionSiblings.Append(parent);
                }
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

        var connection = locationRegion.Connections.FirstOrDefault(c => c.Regions.Any(r => r == destinationRegion));
        if (connection == null)
        {
            if (!allowDestinationChild)
            {
                return false;
            }

            var destinationRegionChildren = regionsByParentId[destination.RegionId];
            if (!destinationRegionChildren.Any())
            {
                return false;
            }

            connection = locationRegion.Connections.FirstOrDefault(c => c.Regions.Intersect(destinationRegionChildren).Any());
            if (connection == null)
            {
                return false;
            }
        }

        return CanTraverseConnection(unit, connection);
    }

    public bool EqualsOrIsRelated(Location location1, Location location2)
    {
        var location1Region = regionById[location1.RegionId];
        var location2Region = regionById[location2.RegionId];

        Region? location1ParentRegion = null;
        Region? location2ParentRegion = null;
        if (location1Region.ParentId != null && regionById.TryGetValue(location1Region.ParentId, out var l1pr))
        {
            location1ParentRegion = l1pr;
        }

        if (location2Region.ParentId != null && regionById.TryGetValue(location2Region.ParentId, out var l2pr))
        {
            location2ParentRegion = l2pr;
        }

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
                Timeline = location1.Timeline,
                Year = location1.Year,
                Phase = location1.Phase,
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

    public List<Location> GetAdjacentRegions(Unit unit)
    {
        var location = unit.Location;

        var locationRegion = regionById[location.RegionId];
        var adjacentRegions = locationRegion.Connections
            .Where(c => CanTraverseConnection(unit, c))
            .Select(c => c.Regions.First(r => r != locationRegion));

        return [.. adjacentRegions.Select(r => new Location
        {
            Timeline = location.Timeline,
            Year = location.Year,
            Phase = location.Phase,
            RegionId = r.Id,
        })];
    }

    private bool CanTraverseConnection(Unit unit, Connection connection)
    {
        var isValidArmyMove = unit.Type == UnitType.Army && connection.Type != ConnectionType.Sea;
        var isValidFleetMove = unit.Type == UnitType.Fleet && connection.Type != ConnectionType.Land;
        return isValidArmyMove || isValidFleetMove;
    }
}

