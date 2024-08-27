using Entities;
using Enums;
using System.Text.Json;
using Utilities;

namespace Factories;

public class MapFactory
{
    private const string RegionsFilePath = "Data/regions.json";
    private const string ConnectionsFilePath = "Data/connections.json";

    public List<Region> CreateRegions()
    {
        var regionsFile = File.ReadAllText(RegionsFilePath);
        var regions = JsonSerializer.Deserialize<List<Region>>(regionsFile, Constants.JsonOptions)
            ?? throw new JsonException("Deserialised regions file was null");

        var connectionsFile = File.ReadAllText(ConnectionsFilePath);
        var connections = JsonSerializer.Deserialize<List<Connection>>(connectionsFile, Constants.JsonOptions)
            ?? throw new JsonException("Deserialised connections file was null");

        Check(regions, connections);
        PopulateConnections(regions, connections);
        return regions;
    }

    private void PopulateConnections(List<Region> regions, List<Connection> connections)
    {
        foreach (var connection in connections)
        {
            var regionIds = connection.Id.Split("-");

            var startRegion = regions.First(r => r.Id == regionIds[0]);
            var endRegion = regions.First(r => r.Id == regionIds[1]);

            startRegion.Connections.Add(connection);
            endRegion.Connections.Add(connection);

            connection.Regions.Add(startRegion);
            connection.Regions.Add(endRegion);
        }

        foreach (var region in regions)
        {
            if (region.ParentId == null)
            {
                continue;
            }

            var parent = regions.First(r => r.Id == region.ParentId);
            parent.Children.Add(region);
            region.Parent = parent;
        }
    }

    private void Check(List<Region> regions, List<Connection> connections)
    {
        foreach (var region in regions)
        {
            var regionConnections = connections.Where(c => c.Id.Split("-").Contains(region.Id));

            if (regions.Where(r => r.Id == region.Id).Count() > 1)
            {
                throw new JsonException($"Non-unique ID {region.Id} found");
            }

            if (regions.Where(r => r.Name == region.Name).Count() > 1)
            {
                throw new JsonException($"Non-unique name {region.Name} found");
            }

            if (!regionConnections.Any())
            {
                throw new JsonException($"Unconnected region {region.Id} found");
            }

            if (region.Type == RegionType.Land && regionConnections.Any(c => c.Type != ConnectionType.Land))
            {
                throw new JsonException($"Land region {region.Id} with non-land connection found");
            }

            if (region.Type == RegionType.Sea && regionConnections.Any(c => c.Type != ConnectionType.Sea))
            {
                throw new JsonException($"Sea region {region.Id} with non-sea connection found");
            }

            if (region.Type == RegionType.Coast && regionConnections.All(c => c.Type != ConnectionType.Sea))
            {
                throw new JsonException($"Coast region {region.Id} has no sea connection");
            }
        }

        foreach (var connection in connections)
        {
            var connectionRegions = regions.Where(r => connection.Id.Split("-").Contains(r.Id));

            if (connections.Where(c => c.Id == connection.Id).Count() > 1)
            {
                throw new JsonException($"Non-unique ID {connection.Id} found");
            }

            if (connections.Any(c => c.Id.Split("-")[0] == connection.Id.Split("-")[1] && c.Id.Split("-")[1] == connection.Id.Split("-")[0]))
            {
                throw new JsonException($"Duplicate reverse connection for {connection.Id} found");
            }

            if (connectionRegions.Count() != 2)
            {
                throw new JsonException($"Invalid region count for connection {connection.Id}");
            }

            if (connectionRegions.First().Id == connectionRegions.Last().Id)
            {
                throw new JsonException($"Loop connection ${connection.Id} found");
            }

            if (connection.Type == ConnectionType.Sea && connectionRegions.All(r => r.Type != RegionType.Sea && r.ParentId == null))
            {
                throw new JsonException($"Sea connection {connection.Id} without sea or child coast region found");
            }
        }
    }
}

