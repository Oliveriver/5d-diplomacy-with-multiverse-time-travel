﻿using Entities;
using Enums;

namespace Adjudication;

public class MovementEvaluator(World world, List<Order> activeOrders, List<Region> regions, AdjacencyValidator adjacencyValidator)
{
    private readonly World world = world;
    private readonly List<Order> activeOrders = activeOrders;

    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;

    public void EvaluateMovements()
    {
        LinkSupports();

        var orderSetResolver = new OrderSetResolver(world, activeOrders, regions, adjacencyValidator);
        orderSetResolver.RunResolutionAlgorithm();

        IdentifyRetreats();
    }

    private void LinkSupports()
    {
        var supports = activeOrders.OfType<Support>().Where(s => s.Status != OrderStatus.Invalid).ToList();

        foreach (var support in supports)
        {
            var supportedOrder = activeOrders.First(o => o.Location == support.Midpoint);
            supportedOrder.Supports.Add(support);
        }
    }

    private void IdentifyRetreats()
    {
        var holds = world.Orders.OfType<Hold>();
        var moves = world.Orders.OfType<Move>();
        var supports = world.Orders.OfType<Support>();
        var convoys = world.Orders.OfType<Convoy>();

        List<Order> stationaryOrders = [.. holds, .. supports, .. convoys, .. moves.Where(m => m.Status is OrderStatus.Failure or OrderStatus.Invalid)];

        foreach (var order in activeOrders)
        {
            var unit = order.Unit;

            var existingRetreats = world.Orders
                .Where(o =>
                    o.Unit == unit
                    && o.Status is OrderStatus.RetreatNew
                    or OrderStatus.RetreatSuccess
                    or OrderStatus.RetreatFailure
                    or OrderStatus.RetreatInvalid)
                .ToList();

            foreach (var existingRetreat in existingRetreats)
            {
                world.Orders.Remove(existingRetreat);
            }

            var isSuccessfulMove = order is Move && order.Status == OrderStatus.Success;
            var mustRetreat = !isSuccessfulMove && activeOrders.Any(o =>
                o is Move m
                && adjacencyValidator.EqualsOrIsRelated(m.Destination, order.Location)
                && m.Status == OrderStatus.Success);

            if (!mustRetreat)
            {
                continue;
            }

            var canEscape = CanEscape(unit, stationaryOrders, moves.ToList());

            if (canEscape)
            {
                unit.MustRetreat = true;
                continue;
            }

            var disband = new Disband
            {
                Status = OrderStatus.RetreatSuccess,
                Unit = unit,
                UnitId = unit.Id,
                Location = unit.Location,
            };
            world.Orders.Add(disband);
        }
    }

    private bool CanEscape(Unit unit, List<Order> stationaryOrders, List<Move> moves)
    {
        var escapeRoutes = adjacencyValidator.GetAdjacentRegions(unit);

        foreach (var escapeRoute in escapeRoutes)
        {
            var successfulHold = stationaryOrders.FirstOrDefault(o => o.Unit != unit && adjacencyValidator.EqualsOrIsRelated(o.Location, escapeRoute));

            var incomingMoves = moves.Where(m => adjacencyValidator.EqualsOrIsRelated(m.Destination, escapeRoute));

            var successfulIncomingMove = incomingMoves.FirstOrDefault(m => m.Status == OrderStatus.Success);
            var bouncedIncomingMoves = incomingMoves
                .Where(m => m.Status == OrderStatus.Failure && m.ConvoyPath.All(c => c.Status == OrderStatus.Success));

            var isDestinationOccupied = successfulHold != null || successfulIncomingMove != null;
            var hadBounceInDestination = bouncedIncomingMoves.Count() >= 2;
            var hasOpposingMove = moves.Any(m =>
                m.Status == OrderStatus.Success
                && adjacencyValidator.EqualsOrIsRelated(m.Location, escapeRoute)
                && adjacencyValidator.EqualsOrIsRelated(m.Destination, unit.Location)
                && m.ConvoyPath.Count == 0);

            if (!isDestinationOccupied && !hadBounceInDestination && !hasOpposingMove)
            {
                return true;
            }
        }

        return false;
    }
}

