using Entities;
using Enums;

namespace Adjudication;

public class AdjacencyValidator(List<Region> regions, bool hasStrictAdjacencies)
{
    private readonly List<Region> regions = regions;
    private readonly bool hasStrictAdjacencies = hasStrictAdjacencies;

    public bool IsValidDirectMove(Unit unit, Location location, Location destination)
    {
        if (location.Phase == Phase.Winter || destination.Phase == Phase.Winter)
        {
            return false;
        }

        var isSameBoard = location.Timeline == destination.Timeline
            && location.Year == destination.Year
            && location.Phase == destination.Phase;

        return isSameBoard
            ? IsValidIntraBoardMove(unit, location, destination)
            : !unit.MustRetreat && IsValidInterBoardMove(unit, location, destination);
    }

    public bool IsValidInterBoardMove(Unit unit, Location location, Location destination)
    {
        var locationId = location.RegionId;
        var destinationId = destination.RegionId;

        if (hasStrictAdjacencies ? locationId != destinationId : !IsValidIntraBoardMove(unit, location, destination))
        {
            return false;
        }

        var yearDistance = location.Year - destination.Year;
        var phaseDistance = (int)location.Phase - (int)destination.Phase;
        var timeDistance = Math.Abs(2 * yearDistance + phaseDistance);

        var multiverseDistance = Math.Abs(location.Timeline - destination.Timeline);

        return timeDistance <= 1 && multiverseDistance <= 1 && (timeDistance == 0 || multiverseDistance == 0);
    }

    public bool IsValidIntraBoardMove(Unit unit, Location location, Location destination)
    {
        var locationId = location.RegionId;
        var destinationId = destination.RegionId;

        var region = regions.First(r => r.Id == locationId);

        var connection = region.Connections.FirstOrDefault(c => c.Regions.Any(r => r.Id == destinationId));
        if (connection == null)
        {
            return false;
        }

        var isValidArmyMove = unit.Type == UnitType.Army && connection.Type != ConnectionType.Sea;
        var isValidFleetMove = unit.Type == UnitType.Fleet && connection.Type != ConnectionType.Land;
        return isValidArmyMove || isValidFleetMove;
    }
}

