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

    public DbSet<Tourist> Tourists { get; set; }

    public DbSet<Wallet> Wallets { get; set; }

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

        modelBuilder.Entity<Club>()
            .Property(c => c.Status)
            .HasConversion<int>();

        modelBuilder.Entity<Club>()
            .Property(c => c.MemberIds)
            .HasConversion(
                v => v == null || v.Count == 0 ? "" : string.Join(',', v),
                v => string.IsNullOrWhiteSpace(v)
                    ? new List<long>()
                    : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(long.Parse)
                         .ToList()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<long>>(
                (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c == null ? new List<long>() : c.ToList()
            ));


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


        // Konfiguracija za Tourist - SAMOSTALNA tabela (NE TPH!)
        modelBuilder.Entity<Tourist>(entity =>
        {
            entity.HasKey(t => t.Id);

            // Foreign key ka Person tabeli
            entity.HasOne(t => t.Person)
                .WithOne()
                .HasForeignKey<Tourist>(t => t.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            // Konverzija liste EquipmentIds u string
            entity.Property(t => t.EquipmentIds)
                .HasConversion(
                    ids => ids == null || ids.Count == 0 ? "" : string.Join(',', ids),
                    ids => string.IsNullOrWhiteSpace(ids)
                        ? new List<long>()
                        : ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(long.Parse)
                             .ToList()
                )
                .HasColumnName("EquipmentIds")
                .IsRequired(false);

            // VALUE COMPARER
            entity.Property(t => t.EquipmentIds)
                .Metadata.SetValueComparer(
                    new ValueComparer<List<long>>(
                        (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                        c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c == null ? new List<long>() : c.ToList()
                    )
                );
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(w => w.Id);
            entity.HasIndex(w => w.PersonId).IsUnique();
            entity.Property(w => w.BalanceAc).IsRequired();
        });

    }

    private static void ConfigureStakeholder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);
    }
}