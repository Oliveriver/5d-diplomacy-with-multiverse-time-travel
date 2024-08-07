using Entities;
using Enums;
using Factories;

namespace Adjudication;

public class Validator(DefaultWorldFactory defaultWorldFactory)
{
    private readonly DefaultWorldFactory defaultWorldFactory = defaultWorldFactory;

    private AdjacencyValidator adjacencyValidator = null!;
    private ConvoyPathValidator convoyPathValidator = null!;
    private World world = null!;
    private List<Region> regions = null!;

    public void Validate(World world, List<Region> regions, bool hasStrictAdjacencies)
    {
        this.world = world;
        this.regions = regions;
        adjacencyValidator = new(regions, hasStrictAdjacencies);
        convoyPathValidator = new(world, regions, adjacencyValidator);

        ValidateMoves();
        ValidateSupports();
        ValidateConvoys();
        ValidateBuilds();
        ValidateDisbands();
        ValidateRetreats();
    }

    private void ValidateMoves()
    {
        var moves = world.Orders.Where(o => o.NeedsValidation && !o.Unit!.MustRetreat).OfType<Move>();

        foreach (var move in moves)
        {
            var canDirectMove = adjacencyValidator.IsValidDirectMove(move.Unit!, move.Location, move.Destination);
            var canConvoyMove = convoyPathValidator.HasPath(move.Unit!, move.Location, move.Destination);

            move.Status = canDirectMove || canConvoyMove ? OrderStatus.New : OrderStatus.Invalid;
        }
    }

    private void ValidateSupports()
    {
        var supports = world.Orders.Where(o => o.NeedsValidation && !o.Unit!.MustRetreat).OfType<Support>();

        foreach (var support in supports)
        {
            var canSupport = adjacencyValidator.IsValidDirectMove(support.Unit!, support.Location, support.Destination);
            var hasMatchingMove = world.Orders
                .OfType<Move>()
                .Any(m => m.Location == support.Midpoint && m.Destination == support.Destination);

            support.Status = canSupport && hasMatchingMove ? OrderStatus.New : OrderStatus.Invalid;
        }
    }

    private void ValidateConvoys()
    {
        var convoys = world.Orders.Where(o => o.NeedsValidation && !o.Unit!.MustRetreat).OfType<Convoy>();

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
                .Any(m => m.Location == convoy.Midpoint && m.Destination == convoy.Destination);

            convoy.Status = hasMatchingMove ? OrderStatus.New : OrderStatus.Invalid;
        }
    }

    private void ValidateBuilds()
    {
        var builds = world.Orders.Where(o => o.NeedsValidation && !o.Unit!.MustRetreat).OfType<Build>();
        var homeCentres = defaultWorldFactory.CreateCentres();

        foreach (var build in builds)
        {
            if (build.Location.Phase != Phase.Winter)
            {
                build.Status = OrderStatus.Invalid;
                continue;
            }

            var board = world.Boards
                .FirstOrDefault(b => b.Timeline == build.Location.Timeline && b.Year == build.Location.Year && b.Phase == Phase.Winter);
            var region = regions.First(r => r.Id == build.Location.RegionId);
            var centre = homeCentres.FirstOrDefault(c => c.Location.RegionId == build.Location.RegionId);
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

            // NB validation based on available build count to be done as part of adjudication
        }
    }

    private void ValidateDisbands()
    {
        var disbands = world.Orders.Where(o => o.NeedsValidation && !o.Unit!.MustRetreat).OfType<Disband>();

        foreach (var disband in disbands)
        {
            disband.Status = disband.Location.Phase != Phase.Winter ? OrderStatus.New : OrderStatus.Invalid;

            // NB validation based on available build count to be done as part of adjudication
        }
    }

    private void ValidateRetreats()
    {
        var retreats = world.Orders.Where(o => o.NeedsValidation && o.Unit!.MustRetreat);

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
