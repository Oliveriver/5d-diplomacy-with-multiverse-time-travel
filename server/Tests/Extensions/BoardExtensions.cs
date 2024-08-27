using Entities;
using Enums;
using FluentAssertions;
using Utilities;

namespace Tests;

internal static class BoardExtensions
{
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

