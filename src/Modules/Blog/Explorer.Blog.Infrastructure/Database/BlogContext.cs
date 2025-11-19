using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database;

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options) : base(options) {}

    public DbSet<Facility> Facilities { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("blog");
    }
}