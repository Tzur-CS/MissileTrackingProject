using Microsoft.EntityFrameworkCore;
using MissileTracking.Models;

namespace MissileTracking.Database
{
    /// <summary>
    /// EF Core DbContext that represents the real database.
    /// </summary>
    public class MissileDbContext : DbContext
    {
        public MissileDbContext(DbContextOptions<MissileDbContext> options) : base(options)
        {
        }

        public DbSet<MissileInfo> Missiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MissileInfo>().HasKey(m => m.Id);
            modelBuilder.Entity<MissileInfo>().Property(m => m.IsIntercepted).HasDefaultValue(false);
            modelBuilder.Entity<MissileInfo>().Property(m => m.InterceptSuccess).HasDefaultValue(false);
        }
    }
}