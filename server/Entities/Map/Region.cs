using Enums;

namespace Entities;

public sealed record Region(string Id, string Name, RegionType Type, Region? Parent);
