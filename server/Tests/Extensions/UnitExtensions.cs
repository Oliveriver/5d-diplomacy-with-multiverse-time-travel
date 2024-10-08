﻿using Entities;
using Enums;

namespace Tests;

internal static class UnitExtensions
{
    public static Unit Get(this List<Unit> units, string regionId, int timeline = 1, int year = 1901, Phase phase = Phase.Spring)
    {
        var location = new Location
        {
            Timeline = timeline,
            Year = year,
            Phase = phase,
            RegionId = regionId,
        };

        return units.First(u => u.Location == location);
    }

    public static Hold Hold(this Unit unit, OrderStatus status = OrderStatus.New)
    {
        var world = unit.Board.World;

        if (unit.MustRetreat && status == OrderStatus.New)
        {
            status = OrderStatus.RetreatNew;
        }

        var hold = new Hold
        {
            World = world,
            Status = status,
            Location = unit.Location,
            Unit = unit,
        };

        world.Orders.Add(hold);
        return hold;
    }

    public static Move Move(this Unit unit, string regionId, int timeline = 1, int year = 1901, Phase phase = Phase.Spring, OrderStatus status = OrderStatus.New)
    {
        var world = unit.Board.World;

        if (unit.MustRetreat && status == OrderStatus.New)
        {
            status = OrderStatus.RetreatNew;
        }

        var move = new Move
        {
            World = world,
            Status = status,
            Location = unit.Location,
            Destination = new()
            {
                Timeline = timeline,
                Year = year,
                Phase = phase,
                RegionId = regionId,
            },
            Unit = unit,
        };

        world.Orders.Add(move);
        return move;
    }

    public static Support Support(this Unit unit, Unit supportedUnit, string? regionId = null, int timeline = 1, int year = 1901, Phase phase = Phase.Spring, OrderStatus status = OrderStatus.New)
    {
        var world = unit.Board.World;

        if (unit.MustRetreat && status == OrderStatus.New)
        {
            status = OrderStatus.RetreatNew;
        }

        var support = new Support
        {
            World = world,
            Status = status,
            Location = unit.Location,
            Midpoint = supportedUnit.Location,
            Destination = new()
            {
                Timeline = timeline,
                Year = year,
                Phase = phase,
                RegionId = regionId ?? supportedUnit.Location.RegionId,
            },
            Unit = unit,
        };

        world.Orders.Add(support);
        return support;
    }

    public static Convoy Convoy(this Unit unit, Unit convoyedUnit, string regionId, int timeline = 1, int year = 1901, Phase phase = Phase.Spring, OrderStatus status = OrderStatus.New)
    {
        var world = unit.Board.World;

        if (unit.MustRetreat && status == OrderStatus.New)
        {
            status = OrderStatus.RetreatNew;
        }

        var convoy = new Convoy
        {
            World = world,
            Status = status,
            Location = unit.Location,
            Midpoint = convoyedUnit.Location,
            Destination = new()
            {
                Timeline = timeline,
                Year = year,
                Phase = phase,
                RegionId = regionId,
            },
            Unit = unit,
        };

        world.Orders.Add(convoy);
        return convoy;
    }

    public static Disband Disband(this Unit unit, OrderStatus status = OrderStatus.New)
    {
        var world = unit.Board.World;

        if (unit.MustRetreat && status == OrderStatus.New)
        {
            status = OrderStatus.RetreatNew;
        }

        var disband = new Disband
        {
            World = world,
            Status = status,
            Location = unit.Location,
            Unit = unit,
        };

        world.Orders.Add(disband);
        return disband;
    }
}
