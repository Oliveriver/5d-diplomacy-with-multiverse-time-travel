using Enums;

namespace Models;

public record GameLoadRequest(bool IsSandbox, Nation? Player, IFormFile File);
