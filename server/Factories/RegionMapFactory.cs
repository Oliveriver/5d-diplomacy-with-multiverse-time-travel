using System.Text.Json;
using Entities;
using Enums;
using Utilities;

namespace Factories;

public class RegionMapFactory
{
    private const string RegionsFilePath = "Data/regions.json";
    private const string ConnectionsFilePath = "Data/connections.json";

    private sealed record RegionJson(string Id, string Name, RegionType Type, string? ParentId);
    private sealed record ConnectionJson(string Id, ConnectionType Type);

    private readonly object syncObject = new();
    private RegionMap? map;

    public RegionMap CreateMap()
    {
        lock (syncObject)
        {
            map ??= CreateMapInternal();
        }

        return map;
    }

    private static RegionMap CreateMapInternal()
    {
        var regionsFile = File.ReadAllText(RegionsFilePath);
        var regionsJson = JsonSerializer.Deserialize<RegionJson[]>(regionsFile, Constants.JsonOptions)
            ?? throw new JsonException("Deserialised regions file was null");

        var connectionsFile = File.ReadAllText(ConnectionsFilePath);
        var connectionsJson = JsonSerializer.Deserialize<ConnectionJson[]>(connectionsFile, Constants.JsonOptions)
            ?? throw new JsonException("Deserialised connections file was null");

        var duplicateRegionIds = regionsJson.Select(x => x.Id).Duplicates().ToList();
        if (duplicateRegionIds.Count > 0)
        {
            throw new JsonException($"Non-unique region IDs found: {string.Join(", ", duplicateRegionIds)}");
        }

        var duplicateRegionNames = regionsJson.Select(x => x.Name).Duplicates().ToList();
        if (duplicateRegionNames.Count > 0)
        {
            throw new JsonException($"Non-unique region names found: {string.Join(", ", duplicateRegionNames)}");
        }

        var regionById = new Dictionary<string, Region>(regionsJson.Length);
        var rootRegionById = new Dictionary<string, Region>(regionsJson.Length);
        foreach (var region in regionsJson)
        {
            if (region.ParentId == null)
            {
                var r = new Region(region.Id, region.Name, region.Type, null);
                rootRegionById.Add(region.Id, r);
                regionById.Add(region.Id, r);
            }
        }

        foreach (var region in regionsJson)
        {
            if (region.ParentId != null)
            {
                if (!rootRegionById.TryGetValue(region.ParentId, out var parent))
                {
                    throw new JsonException($"Region {region.Id} has parent {region.ParentId}, which either cannot be found or is an invalid parent because it itself has a parent");
                }

                regionById.Add(region.Id, new Region(region.Id, region.Name, region.Type, parent));
            }
        }

        var duplicateConnectionIds = connectionsJson.Select(x => x.Id).Duplicates().ToList();
        if (duplicateConnectionIds.Count > 0)
        {
            throw new JsonException($"Non-unique connection IDs found: {string.Join(", ", duplicateConnectionIds)}");
        }

        var connections = connectionsJson.Select(connection =>
        {
            var regionIds = connection.Id.Split("-");
            return (connection.Type, Start: regionById[regionIds[0]], End: regionById[regionIds[1]]);
        }).ToArray();

        var loopConnectionIds = connections
            .Where(c => c.Start == c.End)
            .Select(c => $"{c.Start}-{c.End}")
            .ToList();
        if (loopConnectionIds.Count > 0)
        {
            throw new JsonException($"Loop connections found: {string.Join(", ", loopConnectionIds)}");
        }

        var duplicateReverseConnectionIds = connections.Select(c => (c.Start, c.End))
            .Intersect(connections.Select(c => (Start: c.End, End: c.Start)))
            .Select(c => $"{c.Start}-{c.End}")
            .ToList();
        if (duplicateReverseConnectionIds.Count > 0)
        {
            throw new JsonException($"Duplicate reverse connections found: {string.Join(", ", duplicateReverseConnectionIds)}");
        }

        var missingSeaConnections = connections
            .Where(c => c.Type == ConnectionType.Sea && new[] { c.Start, c.End }.All(r => r.Type != RegionType.Sea && r.Parent == null))
            .Select(c => $"{c.Start}-{c.End}")
            .ToList();
        if (missingSeaConnections.Count > 0)
        {
            throw new JsonException($"Sea connections without sea or child coast region found: {string.Join(", ", missingSeaConnections)}");
        }

        var connectionsByRegion = connections
            .Concat(connections.Select(c => (c.Type, Start: c.End, End: c.Start)))
            .ToLookup(c => c.Start, c => new Connection(c.Type, c.End));
        foreach (var region in regionById.Values)
        {
            var connectionsForRegion = connectionsByRegion[region].ToList();
            if (connectionsForRegion.Count == 0)
            {
                throw new JsonException($"Unconnected region {region.Id} found");
            }

            if (region.Type == RegionType.Land && connectionsForRegion.Any(c => c.Type != ConnectionType.Land))
            {
                throw new JsonException($"Land region {region.Id} with non-land connection found");
            }

            if (region.Type == RegionType.Sea && connectionsForRegion.Any(c => c.Type != ConnectionType.Sea))
            {
                throw new JsonException($"Sea region {region.Id} with non-sea connection found");
            }

            if (region.Type == RegionType.Coast && connectionsForRegion.All(c => c.Type != ConnectionType.Sea))
            {
                throw new JsonException($"Coast region {region.Id} has no sea connection");
            }
        }

        return new RegionMap(regionById.Values, connectionsByRegion);
    }
}

