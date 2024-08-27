using Entities;
using Enums;
using Factories;
using Utilities;

namespace Adjudication;

public class Adjudicator
{
    private readonly World world;

    private readonly Validator validator;
    private readonly Evaluator evaluator;
    private readonly Executor executor;

    public Adjudicator(World world, bool hasStrictAdjacencies, MapFactory mapFactory, DefaultWorldFactory defaultWorldFactory)
    {
        this.world = world;

        var regions = mapFactory.CreateRegions();
        var centres = defaultWorldFactory.CreateCentres();
        var adjacencyValidator = new AdjacencyValidator(regions, hasStrictAdjacencies);

        validator = new(world, regions, centres, adjacencyValidator);
        evaluator = new(world, adjacencyValidator);
        executor = new(world, regions);
    }

    public void Adjudicate()
    {
        if (world.Winner != null)
        {
            world.Orders = world.Orders.Where(o => o.Status != OrderStatus.New).ToList();
            return;
        }

        validator.ValidateOrders();
        evaluator.EvaluateOrders();
        executor.ExecuteOrders();

        var winner = GetWinner();
        if (winner != null)
        {
            world.Winner = winner;
        }
    }

    private Nation? GetWinner()
    {
        var activeCentres = world.ActiveBoards.SelectMany(b => b.Centres);

        foreach (var nation in Constants.Nations)
        {
            var ownedCentres = activeCentres.Where(c => c.Owner == nation).DistinctBy(c => c.Location.RegionId);
            if (ownedCentres.Count() >= Constants.VictoryRequiredCentreCount)
            {
                return nation;
            }
        }

        return null;
    }
}
