using Entities;
using Factories;

namespace Tests;

public class AdjudicationTestBase
{
    protected List<Region> Regions { get; private set; }
    protected DefaultWorldFactory DefaultWorldFactory { get; private set; }

    public AdjudicationTestBase()
    {
        DefaultWorldFactory = new();

        var mapFactory = new MapFactory();
        (Regions, _) = mapFactory.CreateMap();
    }
}
