﻿using Entities;
using Enums;

namespace Adjudication;

public class Evaluator
{
    private readonly World world;

    private readonly TouchedOrdersFinder touchedOrdersFinder;

    private readonly MovementEvaluator movementEvaulator;
    private readonly AdjustmentEvaluator adjustmentEvaluator;
    private readonly RetreatEvaluator retreatEvaulator;

    private readonly bool hasRetreats;

    public Evaluator(World world, RegionMap regionMap, AdjacencyValidator adjacencyValidator)
    {
        this.world = world;
        touchedOrdersFinder = new(world, adjacencyValidator);

        hasRetreats = world.HasRetreats();
        var activeOrders = GetActiveOrders();

        movementEvaulator = new(world, activeOrders, regionMap, adjacencyValidator);
        adjustmentEvaluator = new(world, activeOrders);
        retreatEvaulator = new(world, activeOrders, adjacencyValidator);

        foreach (var board in world.ActiveBoards)
        {
            board.MightAdvance = true;
        }
    }

    public void EvaluateOrders()
    {
        if (hasRetreats)
        {
            retreatEvaulator.EvaluateRetreats();
        }
        else
        {
            movementEvaulator.EvaluateMovements();
            adjustmentEvaluator.EvaluateAdjustments();
        }

        SetSafetyFailures();
    }

    private List<Order> GetActiveOrders()
    {
        var newOrders = world.Orders.Where(o => o.Status is OrderStatus.New or OrderStatus.RetreatNew).ToList();

        if (!hasRetreats)
        {
            var idleUnits = world.ActiveBoards
                .Where(b => b.Phase != Phase.Winter)
                .SelectMany(b => b.Units)
                .Where(u => newOrders.All(o => o.Unit != u));

            foreach (var unit in idleUnits)
            {
                var hold = new Hold
                {
                    Status = OrderStatus.New,
                    Unit = unit,
                    UnitId = unit.Id,
                    Location = unit.Location,
                };
                world.Orders.Add(hold);
                newOrders.Add(hold);
            }
        }

        var activeOrders = touchedOrdersFinder.GetTouchedOrders(newOrders, hasRetreats);

        foreach (var order in activeOrders)
        {
            if (!hasRetreats && order.Status != OrderStatus.Invalid)
            {
                order.Status = OrderStatus.New;
            }

            var touchedBoards = world.Boards.Where(b => order.TouchedLocations().Any(l => b.Contains(l))).ToList();
            foreach (var board in touchedBoards)
            {
                board.MightAdvance = true;
            }
        }

        return activeOrders;
    }

    private void SetSafetyFailures()
    {
        foreach (var order in world.Orders)
        {
            if (order.Status == OrderStatus.New)
            {
                order.Status = OrderStatus.Failure;
            }
            else if (order.Status == OrderStatus.RetreatNew)
            {
                order.Status = OrderStatus.RetreatFailure;
            }
        }
    }
}
