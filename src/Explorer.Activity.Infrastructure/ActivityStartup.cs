using Explorer.Activity.Core.Domain.RepositoryInterfaces;
using Explorer.Activity.Infrastructure.Database;
using Explorer.Activity.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Explorer.Activity.Infrastructure
{
    public static class ActivityStartup
    {
        public static IServiceCollection ConfigureActivityModule(this IServiceCollection services)
        {
            var dataSourceBuilder =
                new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("activity"));

            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<ActivityContext>(options =>
                options.UseNpgsql(dataSource));

            services.AddScoped<IUserContentViewRepository, UserContentViewRepository>();

            return services;
        }
    }
}
