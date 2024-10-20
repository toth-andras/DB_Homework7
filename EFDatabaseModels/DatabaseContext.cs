using Microsoft.EntityFrameworkCore;

namespace Homework7.DatabaseModels;

public class DatabaseContext: DbContext
{
    public DbSet<Country> Countries { get; set; }
    public DbSet<Olympic> Olympics { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Result> Results { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(@"Username=student;Password=student;Server=localhost;Database=postgres;Port=5432");
}