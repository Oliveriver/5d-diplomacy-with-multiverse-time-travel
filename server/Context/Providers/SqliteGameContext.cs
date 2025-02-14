using Microsoft.EntityFrameworkCore;

namespace Context;

public class SqliteGameContext(IConfiguration configuration) : GameContext
{
    private readonly IConfiguration configuration = configuration;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite(configuration.GetConnectionString("Sqlite"));
    }
}
