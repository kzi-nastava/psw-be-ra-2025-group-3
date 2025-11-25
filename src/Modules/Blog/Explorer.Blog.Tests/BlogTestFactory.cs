using Explorer.Blog.Infrastructure.Database;
using Explorer.BuildingBlocks.Tests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.Blog.Tests;

public class BlogTestFactory : BaseTestFactory<BlogContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        var database = Environment.GetEnvironmentVariable("DATABASE_SCHEMA") ?? "explorer-v1-test";

        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<BlogContext>));
        services.Remove(descriptor!);
        services.AddDbContext<BlogContext>(SetupTestContext());

        services.AddAuthentication("TestAuth")
           .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", _ => { });

        return services;
    }
}
