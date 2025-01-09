using CarChargingApi.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace CarChargingApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Location> Locations { get; set; }
        public DbSet<ChargePoint> ChargePoints { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasMany(l => l.ChargePoints)
                    .WithOne(c => c.Location)
                    .HasForeignKey(c => c.LocationId);

                entity.Property(l => l.Type)
                    .HasConversion<string>()
                    .IsRequired();
            });

            modelBuilder.Entity<ChargePoint>(entity =>
            {
                entity.Property(c => c.Status)
                    .HasConversion<string>()
                    .IsRequired();
            });
        }
    }
}
