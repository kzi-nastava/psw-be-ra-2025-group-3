using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Core.Mappers;
using Explorer.Blog.Core.UseCases.Administration;
using Explorer.Blog.Core.UseCases;
using Explorer.Blog.Infrastructure.Database;
using Explorer.Blog.Infrastructure.Database.Repositories;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Explorer.Blog.Infrastructure
{
    public static class BlogStartup
    {
        // Glavna metoda koju poziva Blog modul
        public static IServiceCollection ConfigureBlogModule(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(BlogProfile).Assembly);

            SetupCore(services);
            SetupInfrastructure(services);

            return services;
        }

        // --- CORE sloj ---
        private static void SetupCore(IServiceCollection services)
        {
            // ZADRŽANO OBOJE – NIČIJI KOD SE NE GUBI
            services.AddScoped<IFacilityService, FacilityService>();
            services.AddScoped<IBlogService, BlogService>();
        }

        // --- INFRA sloj ---
        private static void SetupInfrastructure(IServiceCollection services)
        {
            // REPOZITORIJUMI – SPOJENA OBA
            services.AddScoped<IFacilityRepository, FacilityDbRepository>();
            services.AddScoped<IBlogRepository, BlogRepository>();

            // DbContext – zadržan ispravan deo
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("blog"));
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<BlogContext>(opt =>
                opt.UseNpgsql(dataSource,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "blog")));
        }
    }
}
