using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.UseCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace Explorer.Stakeholders.Infrastructure.Database;

public class StakeholdersContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Person> People { get; set; }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<AppRating> AppRatings { get; set; }

    
    public DbSet<Club> Clubs { get; set; }
    public DbSet<ClubImage> ClubImages { get; set; }

    public DbSet<Meetup> Meetups { get; set; }

    public DbSet<Preference> Preferences { get; set; }

    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        ConfigureStakeholder(modelBuilder);

        modelBuilder.Entity<Club>()
            .HasMany(c => c.Images)
            .WithOne()
            .HasForeignKey(i => i.ClubId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Club>()
            .HasOne(c => c.FeaturedImage)
            .WithMany()
            .HasForeignKey(c => c.FeaturedImageId)
            .OnDelete(DeleteBehavior.Restrict);


        // Konfiguracija za Preferences
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
    }

    private static void ConfigureStakeholder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);
    }
}