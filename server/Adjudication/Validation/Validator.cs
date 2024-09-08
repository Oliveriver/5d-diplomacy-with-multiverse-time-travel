using Entities;
using Enums;

namespace Adjudication;

public class Validator
{
    private readonly World world;
    private readonly List<Move> moves;
    private readonly List<Support> supports;
    private readonly List<Convoy> convoys;
    private readonly List<Build> builds;
    private readonly List<Disband> disbands;

    private readonly List<Order> nonRetreats;
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

        nonRetreats = world.Orders.Where(o => o.NeedsValidation && !o.Unit!.MustRetreat).ToList();
        retreats = world.Orders.Where(o => o.NeedsValidation && o.Unit!.MustRetreat).ToList();

        moves = nonRetreats.OfType<Move>().ToList();
        supports = nonRetreats.OfType<Support>().ToList();
        convoys = nonRetreats.OfType<Convoy>().ToList();
        builds = nonRetreats.OfType<Build>().ToList();
        disbands = nonRetreats.OfType<Disband>().ToList();

        convoyPathValidator = new(world, convoys, regions, adjacencyValidator);
    }

    public void ValidateOrders()
    {
        if (world.HasRetreats)
        {
            ValidateRetreats();

            foreach (var nonRetreat in nonRetreats)
            {
                nonRetreat.Status = OrderStatus.Invalid;
            }
        }
        else
        {
            ValidateMoves();
            ValidateSupports();
            ValidateConvoys();
            ValidateBuilds();
            ValidateDisbands();
        }
    }

    private void ValidateMoves()
    {
        foreach (var move in moves)
        {
            var canDirectMove = adjacencyValidator.IsValidDirectMove(move.Unit!, move.Location, move.Destination);

            move.ConvoyPath = convoyPathValidator.GetPossibleConvoys(move.Unit!, move.Location, move.Destination);
            var canConvoyMove = move.ConvoyPath.Count > 0;

            move.Status = canDirectMove || canConvoyMove ? OrderStatus.New : OrderStatus.Invalid;
        }

        var existingMoves = world.Orders.OfType<Move>().Except(moves);
        foreach (var move in existingMoves)
        {
            var newConvoyPath = convoyPathValidator.GetPossibleConvoys(move.Unit!, move.Location, move.Destination);
            if (newConvoyPath.Count > 0)
            {
                move.ConvoyPath = newConvoyPath;
                move.Status = OrderStatus.New;
            }
        }
    }

    private void ValidateSupports()
    {
        var stationaryOrders = world.Orders.Where(o =>
            o is Hold or Support or Convoy
            || o is Move m && m.Status == OrderStatus.Invalid && m.Location != m.Destination && !convoyPathValidator.CouldHaveConvoyed(m.Unit!, m.Location, m.Destination));
        var allMoves = world.Orders.OfType<Move>();

        foreach (var support in supports)
        {
            var canSupport = adjacencyValidator.IsValidDirectMove(support.Unit!, support.Location, support.Destination, allowDestinationSibling: true);

            var hasMatchingHold = support.Midpoint == support.Destination
                && stationaryOrders.Any(o => adjacencyValidator.EqualsOrIsRelated(o.Location, support.Destination));

            var hasMatchingMove = allMoves.Any(m =>
                m.Location == support.Midpoint
                && m.Destination == support.Destination
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

            var hasMatchingMove = world.Orders
                .OfType<Move>()
                .Any(m => m.ConvoyPath.Contains(convoy) && m.Status != OrderStatus.Invalid);

            convoy.Status = hasMatchingMove ? OrderStatus.New : OrderStatus.Invalid;
        }
    }

    private void ValidateBuilds()
    {
        var uniqueBuilds = builds.DistinctBy(b => b.Location).ToList();
        var duplicateBuilds = builds.Where(b => !uniqueBuilds.Contains(b)).ToList();

        foreach (var build in uniqueBuilds)
        {
            if (build.Location.Phase != Phase.Winter)
            {
                build.Status = OrderStatus.Invalid;
                continue;
            }

            var board = world.Boards.FirstOrDefault(b => b.Contains(build.Location));
            var region = regions.First(r => r.Id == build.Location.RegionId);
            var parentRegion = regions.FirstOrDefault(r => r.Id == region.ParentId);

            var originalCentre = centres.FirstOrDefault(c =>
                c.Location.RegionId == region.Id
                || c.Location.RegionId == parentRegion?.Id);

            if (board == null || originalCentre == null)
            {
                build.Status = OrderStatus.Invalid;
                continue;
            }

            var isOccupied = board.Units.Any(u => adjacencyValidator.EqualsOrIsRelated(u.Location, build.Location));
            if (isOccupied)
            {
                build.Status = OrderStatus.Invalid;
                continue;
            }

            var currentCentre = board.Centres.First(c => c.Location.RegionId == originalCentre.Location.RegionId);
            var unit = build.Unit!;

            var isCompatibleRegion = originalCentre.Owner == unit.Owner && currentCentre.Owner == unit.Owner;
            var isCompatibleUnit = unit.Type == UnitType.Army && region.Type != RegionType.Sea
                || unit.Type == UnitType.Fleet && region.Type == RegionType.Coast;

            build.Status = isCompatibleRegion && isCompatibleUnit ? OrderStatus.New : OrderStatus.Invalid;
        }

        foreach (var build in duplicateBuilds)
        {
            build.Status = OrderStatus.Invalid;
        }
    }

    private void ValidateDisbands()
    {
        var uniqueDisbands = disbands.DistinctBy(d => d.Location).ToList();
        var duplicateDisbands = disbands.Where(d => !uniqueDisbands.Contains(d)).ToList();

        foreach (var disband in uniqueDisbands)
        {
            disband.Status = disband.Location.Phase == Phase.Winter ? OrderStatus.New : OrderStatus.Invalid;
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
