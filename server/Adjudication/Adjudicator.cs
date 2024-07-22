using Entities;
using Enums;

namespace Adjudication;

public class Adjudicator
{
    public void Adjudicate(World world, List<Region> map)
    {
        foreach (var order in world.Orders.Where(o => o.Status == OrderStatus.New))
        {
            order.Status = OrderStatus.Failure;
        }

        var previousBoard = world.Boards.Last();

        var year = previousBoard.Phase == Phase.Winter ? previousBoard.Year + 1 : previousBoard.Year;
        var phase = previousBoard.Phase switch
        {
            Phase.Spring => Phase.Fall,
            Phase.Fall => Phase.Winter,
            Phase.Winter => Phase.Spring,
            _ => throw new ArgumentOutOfRangeException("Phase not found")
        };

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
    }
}
