using Factories;

namespace Tests;

public abstract class AdjudicationTestBase
{
    protected MapFactory MapFactory { get; private set; }
    protected DefaultWorldFactory DefaultWorldFactory { get; private set; }

    public AdjudicationTestBase()
    {
        MapFactory = new();
        DefaultWorldFactory = new();
    }
}
