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
        var retreats = activeOrders.OfType<Move>().Where(m => m.Unit!.MustRetreat);
        var moves = world.Orders.OfType<Move>();
        var convoys = world.Orders.OfType<Convoy>();

        foreach (var retreat in retreats)
        {
            var board = world.Boards.First(b => b.Contains(retreat.Destination));

            var failedIncomingMoves = moves.Where(m =>
                board.Contains(m.Destination)
                && m.Status == OrderStatus.Failure
                && m.Destination == retreat.Destination
                && convoys.Where(c => c.Midpoint == m.Location && c.Destination == m.Destination)
                    .All(c => c.Status == OrderStatus.Success));

            var isDestinationOccupied = board.Units.Any(u => u.Location == retreat.Destination);
            var hadBounceInDestination = failedIncomingMoves.Any();
            var hasOpposingMove = retreats.Any(r => r != retreat && r.Destination == retreat.Destination);

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
            else
            {
                retreat.Status = OrderStatus.Retreat;
            }

            retreat.Unit!.MustRetreat = false;
        }
    }
}

