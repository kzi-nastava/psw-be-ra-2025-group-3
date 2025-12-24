using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Internal;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.API.Public.Shopping;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Mappers;
using Explorer.Tours.Core.UseCases;
using Explorer.Tours.Core.UseCases.Administration;
using Explorer.Tours.Core.UseCases.Authoring;
using Explorer.Tours.Core.UseCases.Shopping;
using Explorer.Tours.Core.UseCases.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database.Repositories;
using Explorer.Tours.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Core.UseCases.Execution;
using Explorer.Tours.API.Public.Review;
using Explorer.Tours.Core.UseCases.Review;

namespace Explorer.Tours.Infrastructure;

public static class ToursStartup
{
    public static IServiceCollection ConfigureToursModule(this IServiceCollection services)
    {
        // Registers all profiles since it works on the assembly
        services.AddAutoMapper(typeof(ToursProfile).Assembly);
        SetupCore(services);
        SetupInfrastructure(services);
        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IFacilityService, FacilityService>();
        services.AddScoped<IPositionService, PositionService>();
        services.AddScoped<ITourPurchaseTokenService, TourPurchaseTokenService>();
        services.AddScoped<IMonumentService, MonumentService>();
        services.AddScoped<IAwardEventService, AwardEventService>();
        services.AddScoped<ITourProblemService, TourProblemService>();
        services.AddScoped<IInternalEquipmentService, InternalEquipmentService>();
        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        services.AddScoped<IKeyPointService, KeyPointService>();
        services.AddScoped<ITourExecutionService, TourExecutionService>();
        services.AddScoped<ITourReviewService, TourReviewService>();
        services.AddScoped<INotificationService, NotificationService>();

        services.AddScoped<ITouristTourService, TouristTourService>();

        services.AddScoped<ITourAccessService, TourAccessService>();

        services.AddScoped<IAdminTourProblemService, AdminTourProblemService>();

        services.AddScoped<IInternalTourService, InternalTourService>();  //tour recommendations
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IEquipmentRepository, EquipmentDbRepository>();
        services.AddScoped<ITourRepository, TourDbRepository>();
        services.AddScoped<IFacilityRepository, FacilityDbRepository>();
        services.AddScoped<IPositionRepository, PositionDbRepository>();
        services.AddScoped<IMonumentRepository, MonumentDbRepository>();
        services.AddScoped<IAwardEventRepository, AwardEventRepository>();
        services.AddScoped<ITourProblemRepository, TourProblemDbRepository>();
        services.AddScoped<IShoppingCartRepository, ShoppingCartDbRepository>();
        services.AddScoped<ITourPurchaseTokenRepository, TourPurchaseTokenDbRepository>();
        services.AddScoped<IKeyPointRepository, KeyPointDbRepository>();
        services.AddScoped<ITourExecutionRepository, TourExecutionDbRepository>();
        services.AddScoped<ITourReviewRepository, TourReviewDbRepository>();
        services.AddScoped<INotificationRepository, NotificationDbRepository>();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("tours"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ToursContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "tours")));
    }
}