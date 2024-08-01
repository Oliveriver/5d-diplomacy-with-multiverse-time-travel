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
}
