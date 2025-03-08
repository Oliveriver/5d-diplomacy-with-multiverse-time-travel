using Entities;
using Enums;
using Utilities;

namespace Adjudication;

public class AdjustmentEvaluator(World world, List<Order> activeOrders)
{
    private readonly World world = world;
    private readonly List<Order> activeOrders = activeOrders;

    private readonly Random random = new();

    public void EvaluateAdjustments()
    {
        var boards = world.ActiveBoards.Where(b => b.Phase == Phase.Winter);

        foreach (var board in boards)
        {
            board.MightAdvance = true;

            var boardBuilds = activeOrders
                .OfType<Build>()
                .Where(b => b.Status == OrderStatus.New && board.Contains(b.Location)).ToList();
            var boardDisbands = activeOrders
                .OfType<Disband>()
                .Where(d => d.Status == OrderStatus.New && board.Contains(d.Location)).ToList();

            foreach (var nation in Constants.Nations)
            {
                EvaluateBoardForNation(board, nation, boardBuilds, boardDisbands);
            }
        }
    }

    private void EvaluateBoardForNation(Board board, Nation nation, List<Build> boardBuilds, List<Disband> boardDisbands)
    {
        var nationBuilds = boardBuilds.Where(b => b.Unit.Owner == nation).ToList();
        var nationDisbands = boardDisbands.Where(d => d.Unit.Owner == nation).ToList();

        var centreCount = board.Centres.Count(c => c.Owner == nation);
        var unitCount = board.Units.Count(u =>
            !activeOrders.OfType<Build>().Any(o => o.Unit == u)
            && u.Owner == nation);

        var adjustmentCount = centreCount - unitCount;

        RemoveExcessiveBuilds(nationBuilds, adjustmentCount);
        RemoveExcessiveDisbands(nationDisbands, adjustmentCount);
        AddMissingDisbands(board, nation, nationDisbands, adjustmentCount);
    }

    private void RemoveExcessiveBuilds(List<Build> nationBuilds, int adjustmentCount)
    {
        var successes = random.ChooseRandomItems(nationBuilds, adjustmentCount);

        foreach (var build in nationBuilds)
        {
            build.Status = successes.Contains(build) ? OrderStatus.Success : OrderStatus.Failure;
        }
    }

    private void RemoveExcessiveDisbands(List<Disband> nationDisbands, int adjustmentCount)
    {
        var successes = random.ChooseRandomItems(nationDisbands, -adjustmentCount);

        foreach (var disband in nationDisbands)
        {
            disband.Status = successes.Contains(disband) ? OrderStatus.Success : OrderStatus.Failure;
        }
    }

    private void AddMissingDisbands(Board board, Nation nation, List<Disband> nationDisbands, int adjustmentCount)
    {
        var extraDisbandCount = -adjustmentCount - nationDisbands.Count;

        if (extraDisbandCount <= 0)
        {
            return;
        }

        var availableUnits = board.Units.Where(u =>
            u.Owner == nation
            && nationDisbands.All(d => d.Unit != u)).ToList();

        var unitsToDisband = random.ChooseRandomItems(availableUnits, extraDisbandCount);

        foreach (var unit in unitsToDisband)
        {
            world.Orders.Add(new Disband
            {
                Status = OrderStatus.Success,
                Unit = unit,
                UnitId = unit.Id,
                Location = unit.Location,
            });
        }
    }
}

