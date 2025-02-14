using Microsoft.EntityFrameworkCore;

namespace Context;

public class SqlServerGameContext(IConfiguration configuration) : GameContext
{
    private readonly IConfiguration configuration = configuration;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("SqlServer"));
    }
}
