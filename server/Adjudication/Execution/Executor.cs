using Entities;
using Enums;
using Utilities;

namespace Adjudication;

public class Executor(World world, RegionMap regionMap)
{
    private readonly World world = world;
    private readonly HashSet<Unit> originalRetreatingUnits = [.. world.Boards.SelectMany(b => b.Units).Where(u => u.MustRetreat)];

    private readonly RegionMap regionMap = regionMap;
    private readonly MapComparer mapComparer = new([.. world.Orders.OfType<Build>()]);

    public void ExecuteOrders()
    {
        if (world.HasRetreats())
        {
            return;
        }

        var advancingBoards = world.Boards
            .Where(b => b.MightAdvance)
            .OrderBy(b => 3 * b.Year + (int)b.Phase)
            .ThenBy(b => b.Timeline)
            .ToList();

        foreach (var board in advancingBoards)
        {
            board.MightAdvance = false;
            var nextBoard = CreateNextBoard(board);

            var existingNextBoards = world.Boards.Where(b =>
                b.Year == nextBoard.Year
                && b.Phase == nextBoard.Phase
                && (b.Timeline == nextBoard.Timeline || board.ChildTimelines.Contains(b.Timeline)));

            var shouldAdvanceTimeline = !existingNextBoards.Any();
            if (shouldAdvanceTimeline)
            {
                world.Boards.Add(nextBoard);
                continue;
            }

            var shouldSplitTimeline = existingNextBoards.All(b => !mapComparer.Equals(b, nextBoard));
            if (shouldSplitTimeline)
            {
                var nextTimeline = world.Boards.Max(b => b.Timeline) + 1;

                nextBoard.Timeline = nextTimeline;

                foreach (var centre in nextBoard.Centres)
                {
                    centre.Location.Timeline = nextTimeline;
                }

                foreach (var unit in nextBoard.Units)
                {
                    unit.Location.Timeline = nextTimeline;
                }

                board.ChildTimelines.Add(nextTimeline);
                world.Boards.Add(nextBoard);
            }
        }
    }

    private Board CreateNextBoard(Board previousBoard)
        => previousBoard.Phase == Phase.Winter ? AdvanceMinorBoard(previousBoard) : AdvanceMajorBoard(previousBoard);

    private Board AdvanceMajorBoard(Board previousBoard)
    {
        var timeline = previousBoard.Timeline;
        var year = previousBoard.Year;
        var phase = previousBoard.Phase.NextPhase();

        var retreatUnits = world.Orders
            .Where(o => o.Status is OrderStatus.RetreatSuccess or OrderStatus.RetreatFailure or OrderStatus.RetreatInvalid)
            .Select(o => o.Unit)
            .ToHashSet();

        var holds = world.Orders.Where(o =>
            (o is not Move || o.Status != OrderStatus.Success && o.Status != OrderStatus.RetreatSuccess)
            && !retreatUnits.Contains(o.Unit)
            && previousBoard.Contains(o.Location)
            && !originalRetreatingUnits.Contains(o.Unit));
        var incomingMoves = world.Orders.OfType<Move>().Where(m =>
            m.Status is OrderStatus.Success or OrderStatus.RetreatSuccess
            && previousBoard.Contains(m.Destination));

        var units = new List<Unit>();

        foreach (var hold in holds)
        {
            var unit = hold.Unit.Clone();
            unit.Location.Timeline = timeline;
            unit.Location.Year = year;
            unit.Location.Phase = phase;
            units.Add(unit);
        }

        foreach (var move in incomingMoves)
        {
            var unit = move.Unit.Clone();
            unit.Location.Timeline = timeline;
            unit.Location.Year = year;
            unit.Location.Phase = phase;
            unit.Location.RegionId = move.Destination.RegionId;
            units.Add(unit);
        }

        var centres = previousBoard.Centres.Select(c => c.Clone()).ToList();
        foreach (var centre in centres)
        {
            centre.Location.Timeline = timeline;
            centre.Location.Year = year;
            centre.Location.Phase = phase;

            if (phase == Phase.Winter)
            {
                var region = regionMap.GetRegion(centre.Location.RegionId);
                var childRegions = regionMap.GetChildRegions(region.Id);

                var newOwner = units.FirstOrDefault(u =>
                    u.Location.RegionId == region.Id
                    || childRegions.Any(r => u.Location.RegionId == r.Id))?.Owner;

                if (newOwner != null)
                {
                    centre.Owner = newOwner;
                }
            }
        }

        return new()
        {
            Timeline = timeline,
            Year = year,
            Phase = phase,
            ChildTimelines = [],
            Centres = centres,
            Units = units,
        };
    }

    private Board AdvanceMinorBoard(Board previousBoard)
    {
        var timeline = previousBoard.Timeline;
        var year = previousBoard.Year + 1;
        var phase = Phase.Spring;

        var localOrders = world.Orders.Where(o => previousBoard.Contains(o.Location)).ToList();

        var builds = localOrders.OfType<Build>().ToList();
        var unsuccessfulBuildUnits = builds
            .Where(b => b.Status != OrderStatus.Success)
            .Select(b => b.Unit)
            .ToHashSet();
        var successfulDisbandLocations = localOrders.OfType<Disband>()
            .Where(d => d.Status == OrderStatus.Success)
            .Select(d => d.Location)
            .ToHashSet();

        var units = previousBoard.Units
            .Concat(builds.Where(b => b.Status == OrderStatus.Success).Select(b => b.Unit))
            .Where(u =>
                !successfulDisbandLocations.Contains(u.Location)
                && !unsuccessfulBuildUnits.Contains(u))
            .Distinct()
            .Select(u => u.Clone())
            .ToList();

        foreach (var unit in units)
        {
            unit.Location.Timeline = timeline;
            unit.Location.Year = year;
            unit.Location.Phase = phase;
        }

        var centres = previousBoard.Centres.Select(c => c.Clone()).ToList();
        foreach (var centre in centres)
        {
            centre.Location.Timeline = timeline;
            centre.Location.Year = year;
            centre.Location.Phase = phase;
        }

        return new()
        {
            Timeline = timeline,
            Year = year,
            Phase = phase,
            ChildTimelines = [],
            Centres = centres,
            Units = units,
        };
    }
}
