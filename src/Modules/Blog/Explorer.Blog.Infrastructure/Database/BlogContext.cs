using Microsoft.EntityFrameworkCore;
using BlogEntity = Explorer.Blog.Core.Domain.Blogs.Blog;
using BlogImageEntity = Explorer.Blog.Core.Domain.Blogs.BlogImage;

namespace Explorer.Blog.Infrastructure.Database
{
    public class BlogContext : DbContext
    {
        public DbSet<BlogEntity> Blogs { get; set; }
        public DbSet<BlogImageEntity> BlogImages { get; set; }

        public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("blog");

            modelBuilder.Entity<BlogEntity>()
                .HasMany(b => b.Images)
                .WithOne()
                .HasForeignKey(img => img.BlogId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}