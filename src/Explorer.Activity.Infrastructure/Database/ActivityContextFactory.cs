using Explorer.Activity.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace Explorer.Activity.Infrastructure.Database
{
    public class ActivityContextFactory : IDesignTimeDbContextFactory<ActivityContext>
    {
        public ActivityContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ActivityContext>();

            // ⬇️ ISTI connection string koji koriste ostali moduli
            var connectionString =
                "Host=localhost;Database=explorer-v1;Username=postgres;Password=root";

            optionsBuilder.UseNpgsql(connectionString);

            return new ActivityContext(optionsBuilder.Options);
        }
    }
}
