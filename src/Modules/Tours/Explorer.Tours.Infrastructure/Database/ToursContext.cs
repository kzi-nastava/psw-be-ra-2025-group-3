using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace Explorer.Tours.Infrastructure.Database;

public class ToursContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<Tour> Tours { get; set; }

    public DbSet<Monument> Monuments { get; set; }
    public DbSet<Facility> Facilities { get; set; }
    public DbSet<AwardEvent> AwardEvents { get; set; }

    public DbSet<TourProblem> TourProblems { get; set; }

    public DbSet<Position> Positions { get; set; }

    public DbSet<Preference> Preferences { get; set; }

    public DbSet<TouristEquipment> TouristEquipment { get; set; }

    public ToursContext(DbContextOptions<ToursContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");
        modelBuilder.Entity<Tour>().HasIndex(t => t.AuthorId);

        modelBuilder.Entity<Tour>()
            .Property(t => t.Tags)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            ));
        //mapiranje za facilities
        modelBuilder.Entity<Facility>(entity =>
        {
            entity.ToTable("Facilities");

            entity.HasKey(f => f.Id);

            entity.Property(f => f.Id)
                .ValueGeneratedOnAdd();

            entity.Property(f => f.Name)
                .IsRequired();

            entity.Property(f => f.Latitude)
                .IsRequired();

            entity.Property(f => f.Longitude)
                .IsRequired();

            entity.Property(f => f.Category)
                .HasConversion<int>()
                .IsRequired();
        });

        modelBuilder.Entity<AwardEvent>().Property(ae => ae.Name).IsRequired();
        modelBuilder.Entity<AwardEvent>().Property(ae => ae.Description).IsRequired();

        modelBuilder.Entity<AwardEvent>()
            .HasIndex(ae => ae.Year)
            .IsUnique();

        modelBuilder.Entity<AwardEvent>()
            .Property(ae => ae.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Preference>().HasIndex(p => p.TouristId);
        modelBuilder.Entity<Preference>()
            .Property(p => p.Tags)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            ));

        // Konfiguracija za TouristEquipment odnosno many-to-many veza
        modelBuilder.Entity<TouristEquipment>(entity =>
        {
            // Kompozitni primarni ključ (TouristId + EquipmentId)
            entity.HasKey(te => new { te.TouristId, te.EquipmentId });

            // Relacija sa Equipment tabelom
            entity.HasOne(te => te.Equipment)
                .WithMany()
                .HasForeignKey(te => te.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}