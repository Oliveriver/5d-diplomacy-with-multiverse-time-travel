﻿using Entities;
using Enums;

namespace Adjudication;

public class ConvoyPathValidator(List<Convoy> convoys, List<Region> regions, AdjacencyValidator adjacencyValidator)
{
    private readonly List<Convoy> convoys = convoys;

    private readonly List<Region> regions = regions;
    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;

    public List<Convoy> GetPossibleConvoys(Unit unit, Location location, Location destination)
    {
        if (unit.Type == UnitType.Fleet || location == destination)
        {
            return [];
        }

        var startRegion = regions.First(r => r.Id == location.RegionId);
        var endRegion = regions.First(r => r.Id == destination.RegionId);

        var startsOnCoast = startRegion.Type == RegionType.Coast
            || regions.Where(r => r.ParentId == startRegion.Id).Any(r => r.Type == RegionType.Coast);
        var endsOnCoast = endRegion.Type == RegionType.Coast
            || regions.Where(r => r.ParentId == endRegion.Id).Any(r => r.Type == RegionType.Coast);

        if (!startsOnCoast || !endsOnCoast)
        {
            return [];
        }

        var convoysInPath = convoys.Where(c =>
            c.NeedsValidation
            && c.Midpoint == location
            && c.Destination == destination).ToList();

        if (convoysInPath.Count == 0)
        {
            return [];
        }

        var depthFirstSearch = new DepthFirstSearch(convoysInPath, adjacencyValidator, regions);
        return depthFirstSearch.GetPossibleConvoys(unit, location, destination);
    }

    private class DepthFirstSearch(List<Convoy> convoys, AdjacencyValidator adjacencyValidator, List<Region> regions)
    {
        private readonly List<Convoy> convoys = convoys;
        private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;
        private readonly List<Region> regions = regions;

        private readonly List<Convoy> visitedConvoys = [];

        public List<Convoy> GetPossibleConvoys(Unit unit, Location location, Location destination)
        {
            if (adjacencyValidator.IsValidDirectMove(unit, location, destination))
            {
                if (location == convoys.FirstOrDefault()?.Midpoint)
                {
                    var directConvoys = convoys.Where(c =>
                        adjacencyValidator.IsValidDirectMove(c.Unit!, c.Location, destination, allowDestinationChild: true)
                        && adjacencyValidator.IsValidDirectMove(c.Unit!, c.Location, location, allowDestinationChild: true));

                    if (directConvoys.Any())
                    {
                        return directConvoys.ToList();
                    }
                }

                if (unit.Type == UnitType.Fleet)
                {
                    return convoys.Where(c => c.Unit == unit).ToList();
                }
            }

            var convoy = convoys.FirstOrDefault(c => c.Location == location);
            if (convoy != null)
            {
                visitedConvoys.Add(convoy);

                var region = regions.First(r => r.Id == convoy.Location.RegionId);
                if (region.Type != RegionType.Sea)
                {
                    return [];
                }
            }

            var adjacentConvoys = convoys
                .Where(c =>
                    !visitedConvoys.Contains(c)
                    && adjacencyValidator.IsValidDirectMove(c.Unit!, c.Location, location, allowDestinationChild: true))
                .ToList();

            var onwardConvoys = adjacentConvoys
                .SelectMany(c => GetPossibleConvoys(c.Unit!, c.Location, destination))
                .ToList();

            return convoy == null || onwardConvoys.Count == 0 ? [.. onwardConvoys] : [.. onwardConvoys, convoy];
        }
    }
}
