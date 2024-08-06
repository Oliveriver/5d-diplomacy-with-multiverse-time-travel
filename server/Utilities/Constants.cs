using Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Utilities;

public static class Constants
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter(),
        }
    };

    public static readonly List<Nation> Nations = Enum.GetValues(typeof(Nation)).OfType<Nation>().ToList();
}

