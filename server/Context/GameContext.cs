using Entities;
using Microsoft.EntityFrameworkCore;

namespace Context;

public class GameContext(DbContextOptions<GameContext> options) : DbContext(options)
{
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<World> Worlds { get; set; } = null!;
    public DbSet<Board> Boards { get; set; } = null!;
    public DbSet<Centre> Centres { get; set; } = null!;
    public DbSet<Unit> Units { get; set; } = null!;

    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Hold> Holds { get; set; } = null!;
    public DbSet<Move> Moves { get; set; } = null!;
    public DbSet<Support> Supports { get; set; } = null!;
    public DbSet<Convoy> Convoys { get; set; } = null!;
    public DbSet<Build> Builds { get; set; } = null!;
    public DbSet<Disband> Disbands { get; set; } = null!;
};
