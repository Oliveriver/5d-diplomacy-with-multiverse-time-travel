using Enums;

namespace Mappers;

public class EntityMapper
{
    public Models.World MapWorld(Entities.World world)
        => new(world.Iteration, [.. world.Boards.Select(MapBoard)], [.. world.Orders.Select(MapOrder)], world.Winner);

    public Models.Board MapBoard(Entities.Board board)
        => new(board.Timeline, board.Year, board.Phase, [.. board.ChildTimelines], MapCentres(board.Centres), MapUnits(board.Units));

    public Dictionary<string, Nation> MapCentres(List<Entities.Centre> centres)
        => centres.Where(c => c.Owner != null).ToDictionary(c => c.Location.RegionId, c => (Nation)c.Owner!);

    public Dictionary<string, Models.Unit> MapUnits(List<Entities.Unit> units)
        => units.ToDictionary(u => u.Location.RegionId, u => new Models.Unit(u.Owner, u.Type, u.MustRetreat));

    public Models.Order MapOrder(Entities.Order order)
    {
        if (order.Unit == null)
        {
            throw new InvalidOperationException("Order must have an associated unit to be mapped");
        }

        var status = order.Status;
        var unit = MapUnit(order.Unit);
        var location = MapLocation(order.Location);

        return order switch
        {
            Entities.Hold => new Models.Hold(status, unit, location),
            Entities.Move move => new Models.Move(status, unit, location, MapLocation(move.Destination)),
            Entities.Support support => new Models.Support(status, unit, location, MapLocation(support.Midpoint), MapLocation(support.Destination)),
            Entities.Convoy convoy => new Models.Convoy(status, unit, location, MapLocation(convoy.Midpoint), MapLocation(convoy.Destination)),
            Entities.Build => new Models.Build(status, unit, location),
            Entities.Disband => new Models.Disband(status, unit, location),
            _ => throw new ArgumentOutOfRangeException($"Unexpected type {order.GetType()}"),
        };
    }

    public Models.Unit MapUnit(Entities.Unit unit)
        => new(unit.Owner, unit.Type, unit.MustRetreat);

    public Models.Location MapLocation(Entities.Location location)
        => new(location.Timeline, location.Year, location.Phase, location.RegionId);
}

