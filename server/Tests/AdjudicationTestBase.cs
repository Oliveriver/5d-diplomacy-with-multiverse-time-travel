using Factories;

namespace Tests;

public abstract class AdjudicationTestBase
{
    protected RegionMapFactory RegionMapFactory { get; private set; }
    protected DefaultWorldFactory DefaultWorldFactory { get; private set; }

    public AdjudicationTestBase()
    {
        RegionMapFactory = new();
        DefaultWorldFactory = new();
    }
}
