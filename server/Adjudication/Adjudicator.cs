using Entities;
using Enums;
using Utilities;

namespace Adjudication;

public class Adjudicator(Validator validator)
{
    private readonly Validator validator = validator;

    public void Adjudicate(World world, List<Region> regions)
    {
        validator.Validate(world, regions);

        var previousBoard = world.Boards.Last();

        var year = previousBoard.Phase == Phase.Winter ? previousBoard.Year + 1 : previousBoard.Year;
        var phase = previousBoard.Phase.NextPhase();

        world.Boards.Add(new Board
        {
            Timeline = 1,
            Year = year,
            Phase = phase,
            ChildTimelines = [],
            Centres = previousBoard.Centres.Select(c => new Centre
            {
                Owner = c.Owner,
                Location = new()
                {
                    Timeline = 1,
                    Year = year,
                    Phase = phase,
                    RegionId = c.Location.RegionId,
                },
            }).ToList(),
            Units = previousBoard.Units.Select(u => new Unit
            {
                Owner = u.Owner,
                Type = u.Type,
                MustRetreat = u.MustRetreat,
                Location = new()
                {
                    Timeline = 1,
                    Year = year,
                    Phase = phase,
                    RegionId = u.Location.RegionId,
                },
            }).ToList(),
        });

        world.Iteration++;
    }
}
