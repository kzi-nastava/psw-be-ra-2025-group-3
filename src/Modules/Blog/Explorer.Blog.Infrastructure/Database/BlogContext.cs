using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.Blogs;
using Microsoft.EntityFrameworkCore;

using BlogEntity = Explorer.Blog.Core.Domain.Blogs.Blog;
using BlogImageEntity = Explorer.Blog.Core.Domain.Blogs.BlogImage;
using BlogRating = Explorer.Blog.Core.Domain.Blogs.BlogRating;

namespace Explorer.Blog.Infrastructure.Database
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

    
      
        public DbSet<BlogEntity> Blogs { get; set; }
        public DbSet<BlogImageEntity> BlogImages { get; set; }

        public DbSet<Comment> Comments { get; set; }


        public DbSet<BlogRating> BlogRatings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.HasDefaultSchema("blog");

           
           
            modelBuilder.Entity<BlogEntity>()
                .HasMany(b => b.Images)
                .WithOne()
                .HasForeignKey(img => img.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlogEntity>()

                .HasMany(b => b.Comments)
                .WithOne()
                .HasForeignKey(c => c.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

     

        }
    }
}
