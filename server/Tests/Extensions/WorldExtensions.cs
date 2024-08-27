using Entities;
using Enums;

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
}
