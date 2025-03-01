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

    private readonly RegionMap regionMap;
    private readonly Dictionary<string, Centre> originalCentresByRegionId;

    private readonly AdjacencyValidator adjacencyValidator;
    private readonly ConvoyPathValidator convoyPathValidator;

    public Validator(World world, RegionMap regionMap, List<Centre> originalCentres, AdjacencyValidator adjacencyValidator)
    {
        this.world = world;
        this.regionMap = regionMap;
        originalCentresByRegionId = originalCentres.ToDictionary(c => c.Location.RegionId);
        this.adjacencyValidator = adjacencyValidator;

        nonRetreats = [.. world.Orders.Where(o => o.NeedsValidation && !o.Unit.MustRetreat)];
        retreats = [.. world.Orders.Where(o => o.NeedsValidation && o.Unit.MustRetreat)];

        moves = [.. nonRetreats.OfType<Move>()];
        supports = [.. nonRetreats.OfType<Support>()];
        convoys = [.. nonRetreats.OfType<Convoy>()];
        builds = [.. nonRetreats.OfType<Build>()];
        disbands = [.. nonRetreats.OfType<Disband>()];

        convoyPathValidator = new(world, convoys, regionMap, adjacencyValidator);
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
            var canDirectMove = adjacencyValidator.IsValidDirectMove(move.Unit, move.Location, move.Destination);

            move.ConvoyPath = convoyPathValidator.GetPossibleConvoys(move.Unit, move.Location, move.Destination);
            var canConvoyMove = move.ConvoyPath.Count > 0;

            move.Status = canDirectMove || canConvoyMove ? OrderStatus.New : OrderStatus.Invalid;
        }

        var existingMoves = world.Orders.OfType<Move>().Except(moves);
        foreach (var move in existingMoves)
        {
            var newConvoyPath = convoyPathValidator.GetPossibleConvoys(move.Unit, move.Location, move.Destination);
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
            || o is Move m && m.Status == OrderStatus.Invalid && m.Location != m.Destination && !convoyPathValidator.CouldHaveConvoyed(m.Unit, m.Location, m.Destination));
        var allMoves = world.Orders.OfType<Move>();

        foreach (var support in supports)
        {
            var canSupport = adjacencyValidator.IsValidDirectMove(support.Unit, support.Location, support.Destination, allowDestinationSibling: true);

            var hasMatchingHold = support.Midpoint == support.Destination && stationaryOrders.Any(o => o.Location == support.Destination);

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
            var locationRegion = regionMap.GetRegion(convoy.Location.RegionId);
            var midpointRegion = regionMap.GetRegion(convoy.Midpoint.RegionId);
            var destinationRegion = regionMap.GetRegion(convoy.Destination.RegionId);

            var midpointRegionChildren = regionMap.GetChildRegions(midpointRegion.Id);
            var destinationRegionChildren = regionMap.GetChildRegions(destinationRegion.Id);

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
            var region = regionMap.GetRegion(build.Location.RegionId);

            if (board == null || !originalCentresByRegionId.TryGetValue(region.Parent?.Id ?? region.Id, out var originalCentre))
            {
                build.Status = OrderStatus.Invalid;
                continue;
            }

            var isOccupied = board.Units.Any(u => !builds.Any(b => b.Unit == u) && adjacencyValidator.EqualsOrIsRelated(u.Location, build.Location));
            if (isOccupied)
            {
                build.Status = OrderStatus.Invalid;
                continue;
            }

            var currentCentre = board.Centres.First(c => c.Location.RegionId == originalCentre.Location.RegionId);
            var unit = build.Unit;

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
                Move move => adjacencyValidator.IsValidDirectMove(move.Unit, move.Location, move.Destination)
                    ? OrderStatus.RetreatNew
                    : OrderStatus.RetreatInvalid,
                Disband => OrderStatus.RetreatNew,
                _ => OrderStatus.RetreatInvalid,
            };
        }
    }
}
