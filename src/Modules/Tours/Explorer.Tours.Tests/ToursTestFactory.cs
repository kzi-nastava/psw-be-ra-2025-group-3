using Explorer.BuildingBlocks.Tests;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.Tours.Tests;

public class ToursTestFactory : BaseTestFactory<ToursContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        // 1. Remove old DB context registration
        var descriptor = services.SingleOrDefault(d =>
            d.ServiceType == typeof(DbContextOptions<ToursContext>));

        if (descriptor != null)
            services.Remove(descriptor);

        // 2. Register test DB context using SetupTestContext()
        services.AddDbContext<ToursContext>(SetupTestContext());

        return services;
    }
}
