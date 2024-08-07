using Entities;
using Enums;
using Utilities;

namespace Adjudication;

public class Adjudicator(Validator validator)
{
    private readonly Validator validator = validator;

    public void Adjudicate(World world, List<Region> regions, bool hasStrictAdjacencies)
    {
        validator.Validate(world, regions, hasStrictAdjacencies);

        // BEGIN TEMP

        foreach (var order in world.Orders.Where(o => o.Status == OrderStatus.New))
        {
            order.Status = OrderStatus.Success;
        }

        var previousBoard = world.Boards.MaxBy(b => 3 * b.Year + (int)b.Phase)!;

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
            Units = previousBoard.Units.Select(u =>
            {
                var move = world.Orders.OfType<Move>().FirstOrDefault(o => o.Status == OrderStatus.Success && o.Unit == u);

                return new Unit
                {
                    Owner = u.Owner,
                    Type = u.Type,
                    MustRetreat = u.MustRetreat,
                    Location = new()
                    {
                        Timeline = 1,
                        Year = year,
                        Phase = phase,
                        RegionId = move != null ? move.Destination.RegionId : u.Location.RegionId,
                    },
                };
            }).ToList(),
        });

        // END TEMP

        world.Iteration++;
    }
}
