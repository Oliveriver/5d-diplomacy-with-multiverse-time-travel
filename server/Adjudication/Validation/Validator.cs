using Entities;
using Enums;

namespace Adjudication;

public class Validator
{
    private readonly World world;
    private readonly List<Hold> holds;
    private readonly List<Move> moves;
    private readonly List<Support> supports;
    private readonly List<Convoy> convoys;
    private readonly List<Build> builds;
    private readonly List<Disband> disbands;
    private readonly List<Order> retreats;

    private readonly List<Region> regions;
    private readonly List<Centre> centres;

    private readonly AdjacencyValidator adjacencyValidator;
    private readonly ConvoyPathValidator convoyPathValidator;

    public Validator(World world, List<Region> regions, List<Centre> centres, AdjacencyValidator adjacencyValidator)
    {
        this.world = world;
        this.regions = regions;
        this.centres = centres;
        this.adjacencyValidator = adjacencyValidator;

        var nonRetreats = world.Orders.Where(o => o.NeedsValidation && !o.Unit!.MustRetreat).ToList();
        retreats = world.Orders.Where(o => o.NeedsValidation && o.Unit!.MustRetreat).ToList();

        holds = nonRetreats.OfType<Hold>().ToList();
        moves = nonRetreats.OfType<Move>().ToList();
        supports = nonRetreats.OfType<Support>().ToList();
        convoys = nonRetreats.OfType<Convoy>().ToList();
        builds = nonRetreats.OfType<Build>().ToList();
        disbands = nonRetreats.OfType<Disband>().ToList();

        convoyPathValidator = new(convoys, regions, adjacencyValidator);
    }

    public void ValidateOrders()
    {
        ValidateMoves();
        ValidateSupports();
        ValidateConvoys();
        ValidateBuilds();
        ValidateDisbands();
        ValidateRetreats();
    }

    private void ValidateMoves()
    {
        foreach (var move in moves)
        {
            var canDirectMove = adjacencyValidator.IsValidDirectMove(move.Unit!, move.Location, move.Destination);
            var canConvoyMove = convoyPathValidator.HasPath(move.Unit!, move.Location, move.Destination);

            move.Status = canDirectMove || canConvoyMove ? OrderStatus.New : OrderStatus.Invalid;
        }
    }

    private void ValidateSupports()
    {
        var stationaryOrders = new List<Order>();
        stationaryOrders.AddRange(holds);
        stationaryOrders.AddRange(supports);
        stationaryOrders.AddRange(convoys);
        stationaryOrders.AddRange(moves.Where(m => m.Status == OrderStatus.Invalid));

        foreach (var support in supports)
        {
            var canSupport = adjacencyValidator.IsValidDirectMove(support.Unit!, support.Location, support.Destination, allowDestinationSibling: true);

            var hasMatchingHold = support.Midpoint == support.Destination
                && stationaryOrders.Any(o => adjacencyValidator.EqualsOrHasSharedParent(o.Location, support.Destination));

            var hasMatchingMove = moves.Any(m =>
                m.Location == support.Midpoint
                && adjacencyValidator.EqualsOrHasSharedParent(m.Destination, support.Destination)
                && m.Status != OrderStatus.Invalid);

            support.Status = canSupport && (hasMatchingHold || hasMatchingMove) ? OrderStatus.New : OrderStatus.Invalid;
        }
    }

    private void ValidateConvoys()
    {
        foreach (var convoy in convoys)
        {
            var locationRegion = regions.First(r => r.Id == convoy.Location.RegionId);
            var midpointRegion = regions.First(r => r.Id == convoy.Midpoint.RegionId);
            var destinationRegion = regions.First(r => r.Id == convoy.Destination.RegionId);

            var midpointRegionChildren = regions.Where(r => r.ParentId == midpointRegion.Id);
            var destinationRegionChildren = regions.Where(r => r.ParentId == destinationRegion.Id);

            if (locationRegion.Type != RegionType.Sea
                || midpointRegion.Type != RegionType.Coast
                    && (!midpointRegionChildren.Any() || midpointRegionChildren.All(r => r.Type != RegionType.Coast))
                || destinationRegion.Type != RegionType.Coast
                    && (!destinationRegionChildren.Any() || destinationRegionChildren.All(r => r.Type != RegionType.Coast)))
            {
                convoy.Status = OrderStatus.Invalid;
                continue;
            }

            var hasMatchingMove = moves.Any(m => m.Location == convoy.Midpoint && m.Destination == convoy.Destination && m.Status != OrderStatus.Invalid);

            convoy.Status = hasMatchingMove ? OrderStatus.New : OrderStatus.Invalid;
        }
    }

    private void ValidateBuilds()
    {
        foreach (var build in builds)
        {
            if (build.Location.Phase != Phase.Winter)
            {
                build.Status = OrderStatus.Invalid;
                continue;
            }

            var board = world.Boards.FirstOrDefault(b => b.Contains(build.Location));
            var region = regions.First(r => r.Id == build.Location.RegionId);
            var parentRegion = regions.FirstOrDefault(r => r.Id == region.ParentId);

            var centre = centres.FirstOrDefault(c =>
                c.Location.RegionId == region.Id
                || c.Location.RegionId == parentRegion?.Id);
            var unit = build.Unit!;

            if (board == null || centre == null)
            {
                build.Status = OrderStatus.Invalid;
                continue;
            }

            var isCompatibleRegion = centre.Owner == unit.Owner;
            var isCompatibleUnit = unit.Type == UnitType.Army && region.Type != RegionType.Sea
                || unit.Type == UnitType.Fleet && region.Type == RegionType.Coast;

            build.Status = isCompatibleRegion && isCompatibleUnit ? OrderStatus.New : OrderStatus.Invalid;
        }
    }

    private void ValidateDisbands()
    {
        var uniqueDisbands = disbands.DistinctBy(d => d.Location).ToList();
        var duplicateDisbands = disbands.Where(d => !uniqueDisbands.Contains(d)).ToList();

        foreach (var disband in uniqueDisbands)
        {
            disband.Status = disband.Location.Phase != Phase.Winter ? OrderStatus.New : OrderStatus.Invalid;
        }

        foreach (var disband in duplicateDisbands)
        {
            disband.Status = OrderStatus.Invalid;
        }
    }

    private void ValidateRetreats()
    {
        foreach (var retreat in retreats)
        {
            retreat.Status = retreat switch
            {
                Move move => adjacencyValidator.IsValidDirectMove(move.Unit!, move.Location, move.Destination)
                    ? OrderStatus.New
                    : OrderStatus.Invalid,
                Disband => OrderStatus.New,
                _ => OrderStatus.Invalid,
            };
        }
    }
}
