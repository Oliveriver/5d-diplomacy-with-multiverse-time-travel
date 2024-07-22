using Enums;

namespace Models;

public class Location(int timeline, int year, Phase phase, string region)
{
    public int Timeline { get; set; } = timeline;
    public int Year { get; set; } = year;
    public Phase Phase { get; set; } = phase;
    public string Region { get; set; } = region;
}
