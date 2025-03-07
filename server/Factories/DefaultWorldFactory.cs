using Entities;
using Enums;
using System.Text.Json;
using Utilities;

namespace Factories;

public class DefaultWorldFactory
{
    private const string CentresFilePath = "Data/centres.json";
    private const string UnitsFilePath = "Data/units.json";

    private readonly object syncObject = new();
    private Centre[]? centres;
    private Unit[]? units;

    public World CreateWorld()
    {
        var world = new World
        {
            Boards = [CreateBoard()],
            Orders = [],
        };

        Check(world);
        return world;
    }

    private Board CreateBoard() => new()
    {
        Timeline = 1,
        Year = 1901,
        Phase = Phase.Spring,
        ChildTimelines = [],
        Centres = CreateCentres(),
        Units = CreateUnits(),
    };

    public List<Centre> CreateCentres()
    {
        lock (syncObject)
        {
            centres ??= CreateCentresInternal();
        }

        return [.. centres.Select(c => c.Clone())];
    }

    private static Centre[] CreateCentresInternal()
    {
        var centresFile = File.ReadAllText(CentresFilePath);
        var centres = JsonSerializer.Deserialize<Centre[]>(centresFile, Constants.JsonOptions)
            ?? throw new JsonException("Deserialised centres file was null");

        foreach (var centre in centres)
        {
            centre.Location.Timeline = 1;
            centre.Location.Year = 1901;
            centre.Location.Phase = Phase.Spring;
        }

        return centres;
    }

    public List<Unit> CreateUnits()
    {
        lock (syncObject)
        {
            units ??= CreateUnitsInternal();
        }

        return [.. units.Select(c => c.Clone())];
    }

    private static Unit[] CreateUnitsInternal()
    {
        var unitsFile = File.ReadAllText(UnitsFilePath);
        var units = JsonSerializer.Deserialize<Unit[]>(unitsFile, Constants.JsonOptions)
            ?? throw new JsonException("Deserialised units file was null");

        foreach (var unit in units)
        {
            unit.Location.Timeline = 1;
            unit.Location.Year = 1901;
            unit.Location.Phase = Phase.Spring;
        }

        return units;
    }

    private static void Check(World world)
    {
        var centres = world.Boards[0].Centres;
        var units = world.Boards[0].Units;

        var duplicateCentreIds = centres.Select(x => x.Location.RegionId).Duplicates().ToList();
        if (duplicateCentreIds.Count > 0)
        {
            throw new JsonException($"Non-unique centre region IDs found: {string.Join(", ", duplicateCentreIds)}");
        }

        var duplicateUnitIds = units.Select(x => x.Location.RegionId).Duplicates().ToList();
        if (duplicateUnitIds.Count > 0)
        {
            throw new JsonException($"Non-unique unit region IDs found: {string.Join(", ", duplicateUnitIds)}");
        }

        var centreRegions = centres.Select(x => x.Location.RegionId).ToHashSet();
        foreach (var unit in units)
        {
            if (!centreRegions.Contains(unit.Location.RegionId.Split("_")[0]))
            {
                throw new JsonException($"Unit {unit.Location.RegionId} with invalid centre found");
            }
        }
    }
}

