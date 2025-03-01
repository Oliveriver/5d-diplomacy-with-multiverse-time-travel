namespace Entities;

public sealed class RegionMap(IReadOnlyCollection<Region> regions, ILookup<Region, Connection> connectionsByRegion)
{
    private readonly Dictionary<string, Region> regionById = regions.ToDictionary(r => r.Id);
    private readonly ILookup<string, Region> regionsByParentId = regions.Where(r => r.Parent != null).ToLookup(r => r.Parent!.Id);

    public Region GetRegion(string id) => regionById[id];
    public IEnumerable<Region> GetChildRegions(string id) => regionsByParentId[id];
    public IEnumerable<Connection> GetConnections(Region region) => connectionsByRegion[region];

    public IEnumerable<string> RegionIds() => regionById.Keys;
}
