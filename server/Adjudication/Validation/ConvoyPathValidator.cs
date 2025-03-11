using Entities;
using Enums;

namespace Adjudication;

public class ConvoyPathValidator(World world, List<Convoy> convoys, RegionMap regionMap, AdjacencyValidator adjacencyValidator)
{
    private readonly World world = world;
    private readonly List<Convoy> convoys = convoys;

    private readonly RegionMap regionMap = regionMap;
    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;

    public List<Convoy> GetPossibleConvoys(Unit unit, Location location, Location destination)
    {
        if (unit.Type == UnitType.Fleet || location == destination || unit.MustRetreat)
        {
            return [];
        }

        var startRegion = regionMap.GetRegion(location.RegionId);
        var endRegion = regionMap.GetRegion(destination.RegionId);

        var startsOnCoast = startRegion.Type == RegionType.Coast
            || regionMap.GetChildRegions(startRegion.Id).Any(r => r.Type == RegionType.Coast);
        var endsOnCoast = endRegion.Type == RegionType.Coast
            || regionMap.GetChildRegions(endRegion.Id).Any(r => r.Type == RegionType.Coast);

        if (!startsOnCoast || !endsOnCoast)
        {
            return [];
        }

        var successfulConvoysInPath = convoys.Where(c =>
        c.Midpoint == location
        && c.Destination == destination
        && c.Status == OrderStatus.Success).ToList();

        if (successfulConvoysInPath.Count != 0)
        {
            var successfulDepthFirstSearch = new DepthFirstConvoySearch(successfulConvoysInPath, adjacencyValidator, regionMap);
            var successfulConvoyPath = successfulDepthFirstSearch.GetPossibleConvoys(unit, location, destination);
            if (successfulConvoyPath.Count > 0)
            {
                return successfulConvoyPath;
            }
        }

        var convoysInPath = convoys.Where(c =>
            c.Midpoint == location
            && c.Destination == destination).ToList();

        if (convoysInPath.Count == 0)
        {
            return [];
        }

        var depthFirstSearch = new DepthFirstConvoySearch(convoysInPath, adjacencyValidator, regionMap);
        return depthFirstSearch.GetPossibleConvoys(unit, location, destination);
    }

    public bool CouldHaveConvoyed(Unit unit, Location location, Location destination)
    {
        if (unit.Type == UnitType.Fleet || location == destination || unit.MustRetreat)
        {
            return false;
        }

        var startRegion = regionMap.GetRegion(location.RegionId);
        var endRegion = regionMap.GetRegion(destination.RegionId);

        var startsOnCoast = startRegion.Type == RegionType.Coast
            || regionMap.GetChildRegions(startRegion.Id).Any(r => r.Type == RegionType.Coast);
        var endsOnCoast = endRegion.Type == RegionType.Coast
            || regionMap.GetChildRegions(endRegion.Id).Any(r => r.Type == RegionType.Coast);

        if (!startsOnCoast || !endsOnCoast)
        {
            return false;
        }

        var fleets = world.Boards
            .SelectMany(b => b.Units)
            .Where(u => u.Type == UnitType.Fleet)
            .ToList();

        if (fleets.Count == 0)
        {
            return false;
        }

        var depthFirstSearch = new DepthFirstFleetSearch(fleets, adjacencyValidator, regionMap);
        return depthFirstSearch.CouldHaveConvoyed(unit, location, destination);
    }

    private class DepthFirstConvoySearch(List<Convoy> convoys, AdjacencyValidator adjacencyValidator, RegionMap regionMap)
    {
        private readonly List<Convoy> convoys = convoys;
        private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;
        private readonly RegionMap regionMap = regionMap;

        private readonly HashSet<Convoy> visitedConvoys = [];

        public List<Convoy> GetPossibleConvoys(Unit unit, Location location, Location destination)
        {
            if (adjacencyValidator.IsValidDirectMove(unit, location, destination, allowDestinationChild: true))
            {
                if (location == convoys.FirstOrDefault()?.Midpoint)
                {
                    var directConvoys = convoys.Where(c =>
                        adjacencyValidator.IsValidDirectMove(c.Unit, c.Location, destination, allowDestinationChild: true)
                        && adjacencyValidator.IsValidDirectMove(c.Unit, c.Location, location, allowDestinationChild: true));

                    if (directConvoys.Any())
                    {
                        return [.. directConvoys];
                    }
                }

                if (unit.Type == UnitType.Fleet)
                {
                    return [.. convoys.Where(c => c.Unit == unit)];
                }
            }

            var convoy = convoys.FirstOrDefault(c => c.Location == location);
            if (convoy != null)
            {
                visitedConvoys.Add(convoy);

                var region = regionMap.GetRegion(convoy.Location.RegionId);
                if (region.Type != RegionType.Sea)
                {
                    return [];
                }
            }

            var adjacentConvoys = convoys
                .Where(c =>
                    !visitedConvoys.Contains(c)
                    && adjacencyValidator.IsValidDirectMove(c.Unit, c.Location, location, allowDestinationChild: true))
                .ToList();

            var onwardConvoys = adjacentConvoys
                .SelectMany(c => GetPossibleConvoys(c.Unit, c.Location, destination))
                .ToList();

            return convoy == null || onwardConvoys.Count == 0 ? [.. onwardConvoys] : [.. onwardConvoys, convoy];
        }
    }

    private class DepthFirstFleetSearch(List<Unit> fleets, AdjacencyValidator adjacencyValidator, RegionMap regionMap)
    {
        private readonly List<Unit> fleets = fleets;
        private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;
        private readonly RegionMap regionMap = regionMap;

        private readonly HashSet<Unit> visitedFleets = [];

        public bool CouldHaveConvoyed(Unit unit, Location location, Location destination)
        {
            if (adjacencyValidator.IsValidDirectMove(unit, location, destination))
            {
                return true;
            }

            var fleet = fleets.FirstOrDefault(f => f.Location == location);
            if (fleet != null)
            {
                visitedFleets.Add(fleet);

                var region = regionMap.GetRegion(fleet.Location.RegionId);
                if (region.Type != RegionType.Sea)
                {
                    return false;
                }
            }

            var adjacentFleets = fleets
                .Where(f =>
                    !visitedFleets.Contains(f)
                    && adjacencyValidator.IsValidDirectMove(f, f.Location, location, allowDestinationChild: true))
                .ToList();

            var hasOnwardFleets = adjacentFleets.Any(f => CouldHaveConvoyed(f, f.Location, destination));

            return hasOnwardFleets;
        }
    }
}
