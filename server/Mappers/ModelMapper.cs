using Context;
using Microsoft.EntityFrameworkCore;

namespace Mappers;

public class ModelMapper(GameContext context)
{
    private readonly GameContext context = context;

    public async Task<Entities.Order> MapOrder(int gameId, Models.Order order)
    {
        var status = order.Status;
        var location = MapLocation(order.Location);
        var unit = await MapUnit(gameId, location, order.Unit);

        return order switch
        {
            Models.Hold => new Entities.Hold
            {
                Status = status,
                Unit = unit,
                UnitId = unit.Id,
                Location = location,
            },
            Models.Move move => new Entities.Move
            {
                Status = status,
                Unit = unit,
                UnitId = unit.Id,
                Location = location,
                Destination = MapLocation(move.Destination),
            },
            Models.Support support => new Entities.Support
            {
                Status = status,
                Unit = unit,
                UnitId = unit.Id,
                Location = location,
                Midpoint = MapLocation(support.SupportLocation),
                Destination = MapLocation(support.Destination),
            },
            Models.Convoy convoy => new Entities.Support
            {
                Status = status,
                Unit = unit,
                UnitId = unit.Id,
                Location = location,
                Midpoint = MapLocation(convoy.ConvoyLocation),
                Destination = MapLocation(convoy.Destination),
            },
            Models.Build => new Entities.Build
            {
                Status = status,
                Unit = unit,
                UnitId = unit.Id,
                Location = location,
            },
            Models.Disband => new Entities.Disband
            {
                Status = status,
                Unit = unit,
                UnitId = unit.Id,
                Location = location,
            },
            _ => throw new ArgumentOutOfRangeException($"Unexpected type {order.GetType()}"),
        };
    }

    public async Task<Entities.Unit> MapUnit(int gameId, Entities.Location location, Models.Unit unit)
    {
        var existingUnit = await context.Units
            .Where(u => u.Board.World.GameId == gameId)
            .FirstAsync(u => u.Location == location);

        if (existingUnit != null)
        {
            return existingUnit;
        }

        var board = await context.Boards
            .Where(b => b.World.GameId == gameId)
            .FirstAsync(b => b.Timeline == location.Timeline && b.Year == location.Year && b.Phase == location.Phase);

        return new()
        {
            BoardId = board.Id,
            Location = location,
            Owner = unit.Owner,
            Type = unit.Type,
            MustRetreat = unit.MustRetreat,
        };
    }

    public Entities.Location MapLocation(Models.Location location)
        => new()
        {
            Timeline = location.Timeline,
            Year = location.Year,
            Phase = location.Phase,
            RegionId = location.Region,
        };
}

