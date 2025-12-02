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

    public DbSet<TourPurchaseToken>TourPurchaseTokens { get; set; }

    public DbSet<TourProblem> TourProblems { get; set; }

    public DbSet<Position> Positions { get; set; }

    public DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public DbSet<KeyPoint> KeyPoints { get; set; }

    public DbSet<Message> Messages { get; set; }
    public DbSet<TourExecution> TourExecutions { get; set; }

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
            entity.ToTable("Facilities", "tours");   // ⭐ KLJUČNA IZMJENA

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

        
        modelBuilder.Entity<Tour>()
            .HasMany(t => t.Equipment)
            .WithMany()
            .UsingEntity(j => j.ToTable("TourEquipment"));

        modelBuilder.Entity<Tour>()
            .HasMany(t => t.KeyPoints)
            .WithOne()
            .HasForeignKey(kp => kp.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AwardEvent>().Property(ae => ae.Name).IsRequired();
        modelBuilder.Entity<AwardEvent>().Property(ae => ae.Description).IsRequired();

        modelBuilder.Entity<AwardEvent>()
            .HasIndex(ae => ae.Year)
            .IsUnique();

        modelBuilder.Entity<AwardEvent>()
            .Property(ae => ae.Status)
            .HasConversion<string>();

        modelBuilder.Entity<ShoppingCart>(builder =>
        {
            builder.ToTable("ShoppingCarts", "tours");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(c => c.TouristId)
                   .IsRequired();

            builder.Property(c => c.TotalPrice)
                   .IsRequired();

            builder.Property(c => c.Items)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<OrderItem>>(v, (JsonSerializerOptions?)null) ?? new List<OrderItem>()
                )
                .Metadata.SetValueComparer(
                    new ValueComparer<List<OrderItem>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()
                    )
                );
        });

        // Podtask 1 
        modelBuilder.Entity<TourProblem>()
            .HasMany(tp => tp.Messages)           
            .WithOne()                            
            .HasForeignKey("TourProblemId")        // Foreign key u Message tabeli
            .OnDelete(DeleteBehavior.Cascade);     // Brisanje TourProblem-a brise sve Messages

        // Enum konverzije
        modelBuilder.Entity<TourProblem>()
            .Property(tp => tp.Status)
            .HasConversion<int>();

        modelBuilder.Entity<Message>()
            .Property(m => m.AuthorType)
            .HasConversion<int>();
        modelBuilder.Entity<TourExecution>(builder =>
        {
            builder.ToTable("TourExecutions", "tours");
            builder.HasKey(te => te.Id);
            builder.Property(te => te.Id).ValueGeneratedOnAdd();

            builder.Property(te => te.TouristId).IsRequired();
            builder.Property(te => te.TourId).IsRequired();
            builder.Property(te => te.StartTime).IsRequired();
            builder.Property(te => te.Status)
                .HasConversion<int>()
                .IsRequired();
            builder.Property(te => te.StartLatitude).IsRequired();
            builder.Property(te => te.StartLongitude).IsRequired();
            builder.Property(te => te.LastActivity).IsRequired(); // task2

            //  KeyPointCompletion kao JSON
            builder.Property(te => te.CompletedKeyPoints)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<KeyPointCompletion>>(v, (JsonSerializerOptions?)null) ?? new List<KeyPointCompletion>()
                )
                .Metadata.SetValueComparer(
                    new ValueComparer<List<KeyPointCompletion>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()
                    )
                );

            builder.HasIndex(te => new { te.TouristId, te.TourId, te.Status });
        });
    }
}