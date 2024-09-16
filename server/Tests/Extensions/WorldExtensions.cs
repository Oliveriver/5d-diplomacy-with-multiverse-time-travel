using Entities;
using Enums;
using FluentAssertions;

namespace Tests;

internal static class WorldExtensions
{
    public static Board AddBoard(this World world, int timeline = 1, int year = 1901, Phase phase = Phase.Spring)
    {
        var board = new Board
        {
            World = world,
            Timeline = timeline,
            Year = year,
            Phase = phase,
        };

        world.Boards.Add(board);
        return board;
    }

    public static void ShouldHaveAllOrdersResolved(this World world)
    {
        foreach (var order in world.Orders)
        {
            order.Status.Should().BeOneOf(
                OrderStatus.Invalid,
                OrderStatus.Success,
                OrderStatus.Failure,
                OrderStatus.RetreatInvalid,
                OrderStatus.RetreatSuccess,
                OrderStatus.RetreatFailure);
        }
    }
}
