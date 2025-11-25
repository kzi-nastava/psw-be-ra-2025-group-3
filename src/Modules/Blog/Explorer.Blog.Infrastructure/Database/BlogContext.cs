using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

using BlogEntity = Explorer.Blog.Core.Domain.Blogs.Blog;
using BlogImageEntity = Explorer.Blog.Core.Domain.Blogs.BlogImage;

namespace Explorer.Blog.Infrastructure.Database
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

        // ENTITETI
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<BlogEntity> Blogs { get; set; }
        public DbSet<BlogImageEntity> BlogImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Šema
            modelBuilder.HasDefaultSchema("blog");

            // --------------------------
            // FACILITY MAPIRANJE
            // --------------------------
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

            // --------------------------
            // BLOG + IMAGES MAPIRANJE
            // --------------------------
            modelBuilder.Entity<BlogEntity>()
                .HasMany(b => b.Images)
                .WithOne()
                .HasForeignKey(img => img.BlogId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
