using Entities;
using Enums;

namespace Adjudication;

public class RetreatEvaluator(World world, List<Order> activeOrders)
{
    private readonly World world = world;
    private readonly List<Order> activeOrders = activeOrders;

    public void EvaluateRetreats()
    {
        EvaluateDisbands();
        EvaluateRetreatMoves();
        AddMissingDisbands();
    }

    private void EvaluateDisbands()
    {
        var disbands = activeOrders.OfType<Disband>().Where(d => d.Location.Phase != Phase.Winter);
        foreach (var disband in disbands)
        {
            disband.Status = OrderStatus.Retreat;
            disband.Unit!.MustRetreat = false;
        }
    }

    private void EvaluateRetreatMoves()
    {
        var retreats = activeOrders.OfType<Move>().Where(m => m.Unit!.MustRetreat && m.Status != OrderStatus.Invalid).ToList();
        var holds = world.Orders.OfType<Hold>();
        var moves = world.Orders.OfType<Move>();
        var supports = world.Orders.OfType<Support>();
        var convoys = world.Orders.OfType<Convoy>();

        var stationaryOrders = new List<Order>();
        stationaryOrders.AddRange(holds);
        stationaryOrders.AddRange(supports);
        stationaryOrders.AddRange(convoys);
        stationaryOrders.AddRange(moves.Where(m => m.Status == OrderStatus.Invalid));

        foreach (var retreat in retreats)
        {
            var board = world.Boards.First(b => b.Contains(retreat.Destination));

            var successfulHold = stationaryOrders.FirstOrDefault(o => o.Location == retreat.Destination);

            var incomingMoves = moves.Where(m => m.Destination == retreat.Destination);
            var successfulIncomingMove = incomingMoves.FirstOrDefault(m => m.Status == OrderStatus.Success);
            var bouncedIncomingMoves = incomingMoves
                .Where(m => m.Status == OrderStatus.Failure && m.ConvoyPath.All(c => c.Status == OrderStatus.Success));

            var isDestinationOccupied = successfulHold != null || successfulIncomingMove != null;
            var hadBounceInDestination = bouncedIncomingMoves.Count() >= 2;

            var hasOpposingMove = moves.Any(m => m.Location == retreat.Destination && m.Destination == retreat.Location);
            var hasOpposingRetreat = retreats.Any(r => r != retreat && r.Destination == retreat.Destination);

            if (isDestinationOccupied || hadBounceInDestination || hasOpposingMove || hasOpposingRetreat)
            {
                retreat.Status = OrderStatus.Failure;

                var disband = new Disband
                {
                    Status = OrderStatus.Retreat,
                    Unit = retreat.Unit,
                    UnitId = retreat.UnitId,
                    Location = retreat.Location,
                };

                world.Orders.Add(disband);
            }
            else
            {
                retreat.Status = OrderStatus.Retreat;
            }

            retreat.Unit!.MustRetreat = false;
        }
    }

    private void AddMissingDisbands()
    {
        var retreatingUnits = world.ActiveBoards
            .SelectMany(b => b.Units)
            .Where(u => u.MustRetreat);

        foreach (var unit in retreatingUnits)
        {
            var order = activeOrders.FirstOrDefault(o => o.Unit == unit);
            if (order == null || order.Status != OrderStatus.Retreat)
            {
                var disband = new Disband
                {
                    Status = OrderStatus.Retreat,
                    Unit = unit,
                    UnitId = unit.Id,
                    Location = unit.Location,
                };
                world.Orders.Add(disband);

                unit.MustRetreat = false;
            }
        }
    }
}

