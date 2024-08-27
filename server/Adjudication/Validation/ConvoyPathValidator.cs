using Entities;
using Enums;

namespace Adjudication;

public class ConvoyPathValidator(List<Convoy> convoys, List<Region> regions, AdjacencyValidator adjacencyValidator)
{
    private readonly List<Convoy> convoys = convoys;

    private readonly List<Region> regions = regions;
    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;

    public bool HasPath(Unit unit, Location location, Location destination)
    {
        if (unit.Type == UnitType.Fleet || location == destination)
        {
            return false;
        }

        var startRegion = regions.First(r => r.Id == location.RegionId);
        var endRegion = regions.First(r => r.Id == destination.RegionId);

        var startsOnCoast = startRegion.Type == RegionType.Coast
            || regions.Where(r => r.ParentId == startRegion.Id).Any(r => r.Type == RegionType.Coast);
        var endsOnCoast = endRegion.Type == RegionType.Coast
            || regions.Where(r => r.ParentId == endRegion.Id).Any(r => r.Type == RegionType.Coast);

        if (!startsOnCoast || !endsOnCoast)
        {
            return false;
        }

        var convoysInPath = convoys.Where(c =>
            c.NeedsValidation
            && c.Midpoint == location
            && c.Destination == destination).ToList();

        if (convoysInPath.Count == 0)
        {
            return false;
        }

        var depthFirstSearch = new DepthFirstSearch(convoysInPath, adjacencyValidator);
        return depthFirstSearch.HasPath(unit, location, destination);
    }

    private class DepthFirstSearch(List<Convoy> convoys, AdjacencyValidator adjacencyValidator)
    {
        private readonly List<Convoy> convoys = convoys;
        private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;

        private readonly List<Convoy> visitedConvoys = [];

        public bool HasPath(Unit unit, Location location, Location destination)
        {
            if (adjacencyValidator.IsValidDirectMove(unit, location, destination))
            {
                return true;
            }

            var convoy = convoys.FirstOrDefault(c => c.Location == location);
            if (convoy != null)
            {
                visitedConvoys.Add(convoy);
            }

            return convoys
                .Where(c => !visitedConvoys.Contains(c) && adjacencyValidator.IsValidDirectMove(c.Unit!, c.Location, location))
                .Any(c => HasPath(c.Unit!, c.Location, destination));
        }
    }
}
