using Entities;
using Enums;
using FluentAssertions;
using Utilities;

namespace Tests;

internal static class BoardExtensions
{
    public static Build Build(this Board board, Nation owner, UnitType type, string regionId)
    {
        var world = board.World;

        var location = new Location
        {
            Timeline = board.Timeline,
            Year = board.Year,
            Phase = board.Phase,
            RegionId = regionId,
        };
        var build = new Build
        {
            World = world,
            Status = OrderStatus.New,
            Location = location,
            Unit = new()
            {
                Board = board,
                Owner = owner,
                Type = type,
                Location = location,
            },
        };

        world.Orders.Add(build);
        return build;
    }

    public static List<Centre> AddCentres(this Board board, List<(Nation Owner, string RegionId)> centres)
    {
        var createdCentres = centres.Select(c => new Centre
        {
            Board = board,
            Owner = c.Owner,
            Location = new()
            {
                Timeline = board.Timeline,
                Year = board.Year,
                Phase = board.Phase,
                RegionId = c.RegionId,
            },
        }).ToList();

        board.Centres.AddRange(createdCentres);
        return createdCentres;
    }

    public static List<Unit> AddUnits(this Board board, List<(Nation Owner, UnitType Type, string RegionId)> units)
    {
        var createdUnits = units.Select(u => new Unit
        {
            Board = board,
            Owner = u.Owner,
            Type = u.Type,
            Location = new()
            {
                Timeline = board.Timeline,
                Year = board.Year,
                Phase = board.Phase,
                RegionId = u.RegionId,
            },
        }).ToList();

        board.Units.AddRange(createdUnits);
        return createdUnits;
    }

    public static Board Next(this Board board, int? timeline = null)
    {
        var nextTimeline = timeline ?? board.Timeline;
        var nextYear = board.Phase == Phase.Winter ? board.Year + 1 : board.Year;
        var nextPhase = board.Phase.NextPhase();

        return board.World.Boards.First(b =>
            b.Timeline == nextTimeline
            && b.Year == nextYear
            && b.Phase == nextPhase);
    }

    public static void ShouldHaveUnits(this Board board, List<(Nation Owner, UnitType Type, string RegionId, bool MustRetreat)> expectedUnits)
    {
        var actualUnits = board.Units.Select(u => (u.Owner, u.Type, u.Location.RegionId, u.MustRetreat));
        actualUnits.Should().BeEquivalentTo(expectedUnits);
    }

    public static void ShouldNotHaveNextBoard(this Board board, int? timeline = null)
    {
        var nextTimeline = timeline ?? board.Timeline;
        var nextYear = board.Phase == Phase.Winter ? board.Year + 1 : board.Year;
        var nextPhase = board.Phase.NextPhase();

        var boards = board.World.Boards;
        boards.Should().NotContain(b => b.Timeline == nextTimeline && b.Year == nextYear && b.Phase == nextPhase);
    }
}

