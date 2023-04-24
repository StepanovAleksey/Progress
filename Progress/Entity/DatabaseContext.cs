using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace Entity;

public class DatabaseContext:DbContext
{
    public DatabaseContext(DbContextOptions opt)
        : base(opt)
    {
			
    }

    public DbSet<Game> Games { get; set; }
    public DbSet<GameStatistic> GameStatistics { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<BattelField> BattelFields { get; set; }
    public DbSet<UserStatistic> UserStatistics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>();
        modelBuilder.Entity<GameStatistic>();
        modelBuilder.Entity<BattelField>();
        //т.к. Email должен быть уникальным + индекс - для быстрого поиска по почте
        modelBuilder.Entity<User>(o =>
        {
            o.HasIndex(i => i.Email).IsUnique();
            o.Property(p => p.IpAdress).IsRequired(false);
        });
        modelBuilder.Entity<UserStatistic>();
    }
}