using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

        public DbSet<Facility> Facilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Šema
            modelBuilder.HasDefaultSchema("blog");

            // EKSPlicitno mapiranje Facility entiteta
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

                // enum <-> int
                entity.Property(f => f.Category)
                    .HasConversion<int>()   // EF zna da je u bazi int
                    .IsRequired();
            });
        }
    }
}
