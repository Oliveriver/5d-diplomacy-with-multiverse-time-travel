using Enums;

namespace Utilities;

public static class Extensions
{
    public static Phase NextPhase(this Phase phase)
        => phase switch
        {
            Phase.Spring => Phase.Fall,
            Phase.Fall => Phase.Winter,
            Phase.Winter => Phase.Spring,
            _ => throw new ArgumentOutOfRangeException(nameof(phase))
        };

    public static Phase NextMajorPhase(this Phase phase)
        => phase switch
        {
            Phase.Spring => Phase.Fall,
            Phase.Fall => Phase.Spring,
            Phase.Winter => Phase.Spring,
            _ => throw new ArgumentOutOfRangeException(nameof(phase))
        };

    public static List<T> ChooseRandomItems<T>(this Random random, List<T> items, int count)
    {
        if (count >= items.Count)
        {
            return items;
        }

        var chosenItems = new List<T>();

        if (count <= 0)
        {
            return chosenItems;
        }

        while (chosenItems.Count < count)
        {
            var nextIndex = random.Next(0, items.Count);
            var item = items[nextIndex];

            if (!chosenItems.Contains(item))
            {
                chosenItems.Add(item);
            }
        }

        return chosenItems;
    }
}
