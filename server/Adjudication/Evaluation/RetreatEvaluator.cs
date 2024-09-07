using Entities;
using Enums;

namespace Adjudication;

public class RetreatEvaluator(World world, List<Order> activeOrders, AdjacencyValidator adjacencyValidator)
{
    private readonly World world = world;
    private readonly List<Order> activeOrders = activeOrders;

    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;

    public void EvaluateRetreats()
    {
        var boardsWithRetreats = world.Boards.Where(b => b.Units.Any(u => u.MustRetreat));
        foreach (var board in boardsWithRetreats)
        {
            board.MightAdvance = true;
        }

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

            var successfulHold = stationaryOrders.FirstOrDefault(o => adjacencyValidator.EqualsOrIsRelated(o.Location, retreat.Destination));

            var incomingMoves = moves.Where(m => adjacencyValidator.EqualsOrIsRelated(m.Destination, retreat.Destination));

            var successfulIncomingMove = incomingMoves.FirstOrDefault(m => m.Status == OrderStatus.Success);
            var bouncedIncomingMoves = incomingMoves
                .Where(m => m.Status == OrderStatus.Failure && m.ConvoyPath.All(c => c.Status == OrderStatus.Success));

            var isDestinationOccupied = successfulHold != null || successfulIncomingMove != null;
            var hadBounceInDestination = bouncedIncomingMoves.Count() >= 2;
            var hasOpposingMove = moves.Any(m =>
                adjacencyValidator.EqualsOrIsRelated(m.Location, retreat.Destination)
                && adjacencyValidator.EqualsOrIsRelated(m.Destination, retreat.Location)
                && m.ConvoyPath.Count == 0);

            if (isDestinationOccupied || hadBounceInDestination || hasOpposingMove)
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
        }

        var remainingRetreats = retreats.Where(r => r.Status == OrderStatus.New).ToList();

        foreach (var retreat in remainingRetreats)
        {
            var hasOpposingRetreat = remainingRetreats.Any(r => r != retreat && r.Destination == retreat.Destination);
            if (hasOpposingRetreat)
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
        }
    }

    private void AddMissingDisbands()
    {
        var retreatingUnits = world.Boards
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
            }

            unit.MustRetreat = false;
        }
    }
}

