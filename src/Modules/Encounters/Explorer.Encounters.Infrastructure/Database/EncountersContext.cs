using Explorer.Encounters.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace Explorer.Encounters.Infrastructure.Database
{
    public class EncountersContext : DbContext
    {
        public DbSet<Encounter> Encounters { get; set; }
        public EncountersContext(DbContextOptions<EncountersContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("encounters");

            modelBuilder.Entity<Encounter>().HasKey(e => e.Id);

            modelBuilder.Entity<Encounter>()
                .OwnsOne(e => e.Location, location =>
                {
                    location.Property(l => l.Latitude).HasColumnName("Latitude");
                    location.Property(l => l.Longitude).HasColumnName("Longitude");
                });

            modelBuilder.Entity<Encounter>()
                .Property(e => e.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Encounter>()
                .Property(e => e.Type)
                .HasConversion<string>();
        }
    }
}
