using Enums;

namespace Models;

public class Game(int id, Nation? player = null)
{
    public int Id { get; set; } = id;
    public Nation? Player { get; set; } = player;
}
