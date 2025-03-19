namespace Mappers;

public class ModelMapper
{
    public Entities.Order MapOrder(Entities.World world, Models.Order order)
    {
        var status = order.Status;
        var location = MapLocation(order.Location);
        var unit = MapUnit(world, location, order.Unit);

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
            Models.Convoy convoy => new Entities.Convoy
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

    public Entities.Unit MapUnit(Entities.World world, Entities.Location location, Models.Unit unit)
    {
        var board = world.Boards.Single(b => b.Contains(location));
        var existingUnit = board.Units.SingleOrDefault(u => u.Location == location);

        return existingUnit ?? new()
        {
            BoardId = board.Id,
            Board = board,
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

