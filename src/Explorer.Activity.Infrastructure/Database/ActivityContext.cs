using Explorer.Activity.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Activity.Infrastructure.Database
{
    public class ActivityContext : DbContext
    {
        public DbSet<UserContentView> UserContentViews { get; set; }

        public ActivityContext(DbContextOptions<ActivityContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserContentView>(builder =>
            {
                builder.ToTable("UserContentViews", "activity");

                builder.HasKey(x => x.Id);

                builder.Property(x => x.UserId).IsRequired();
                builder.Property(x => x.ContentId).IsRequired();
                builder.Property(x => x.ContentType).HasConversion<int>().IsRequired();
                builder.Property(x => x.ViewedAt).IsRequired();

                builder.HasIndex(x => new { x.UserId, x.ContentType });
            });
        }
    }
}
