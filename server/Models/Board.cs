using Enums;

namespace Models;

public class Board(int timeline, int year, Phase phase, int[] childTimelines, Dictionary<string, Nation> centres, Dictionary<string, Unit> units)
{
    public int Timeline { get; set; } = timeline;
    public int Year { get; set; } = year;
    public Phase Phase { get; set; } = phase;
    public int[] ChildTimelines { get; set; } = childTimelines;
    public Dictionary<string, Nation> Centres { get; set; } = centres;
    public Dictionary<string, Unit> Units { get; set; } = units;
}
