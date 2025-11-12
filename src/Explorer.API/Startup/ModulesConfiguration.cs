using Explorer.Blog.Infrastructure;
using Explorer.Stakeholders.Infrastructure;
using Explorer.Tours.Infrastructure;

namespace Explorer.API.Startup;

public static class ModulesConfiguration
{
    public static IServiceCollection RegisterModules(this IServiceCollection services)
    {
        services.ConfigureStakeholdersModule();
        services.ConfigureToursModule();
        services.ConfigureBlogModule();

        return services;
    }
}