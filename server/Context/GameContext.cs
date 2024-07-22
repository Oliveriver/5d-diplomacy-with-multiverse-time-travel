using Entities;
using Factories;
using Microsoft.EntityFrameworkCore;

namespace Context;

public class GameContext(DbContextOptions<GameContext> options, MapFactory mapFactory) : DbContext(options)
{
    private readonly MapFactory mapFactory = mapFactory;

    public DbSet<Region> Regions { get; set; } = null!;
    public DbSet<Connection> Connections { get; set; } = null!;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var (regions, connections) = mapFactory.CreateMap();

        modelBuilder.Entity<Region>()
            .HasData(regions);

        modelBuilder.Entity<Connection>()
            .HasData(connections);

        modelBuilder.Entity<Region>()
            .HasMany(r => r.Connections)
            .WithMany(c => c.Regions)
            .UsingEntity<ConnectionMapping>("ConnectionMappings")
            .HasData(connections.SelectMany(connection =>
            {
                var regionIds = connection.Id.Split("-");
                return new List<ConnectionMapping>
                {
                    new()
                    {
                        ConnectionsId = connection.Id,
                        RegionsId = regionIds[0],
                    },
                    new()
                    {
                        ConnectionsId = connection.Id,
                        RegionsId = regionIds[1],
                    },
                };
            }));
    }
};
