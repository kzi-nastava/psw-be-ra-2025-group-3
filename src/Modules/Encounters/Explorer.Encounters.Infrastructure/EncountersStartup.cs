using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Explorer.Encounters.Core.Mappers;


namespace Explorer.Encounters.Infrastructure;

public static class EncountersStartup
{
    public static IServiceCollection ConfigureEncountersModule(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(EncountersProfile).Assembly);
        SetupCore(services);
        SetupInfrastructure(services);
        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
        // Register core services here
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        // Register infrastructure services here
    }

}
