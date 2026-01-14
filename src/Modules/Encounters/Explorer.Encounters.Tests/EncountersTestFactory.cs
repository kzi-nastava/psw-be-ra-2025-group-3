using Explorer.BuildingBlocks.Tests;
using Explorer.Encounters.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.Encounters.Tests
{
    public class EncountersTestFactory : BaseTestFactory<EncountersContext>
    {
        protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
        {
            //  Ova metoda redefiniše sve DbContext-e koji su potrebni modulu
            //  (minimalno DbContext od samog modula, kao i DbContext od svakog
            //  modula kog ovaj modul poziva (ako takvih ima)) - zakomentarisano

            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<EncountersContext>));
            services.Remove(descriptor!);
            services.AddDbContext<EncountersContext>(SetupTestContext());

            var toursDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ToursContext>));
            services.Remove(toursDescriptor!);
            services.AddDbContext<ToursContext>(SetupTestContext());

            var stakeholdersDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>));
            services.Remove(stakeholdersDescriptor!);
            services.AddDbContext<StakeholdersContext>(SetupTestContext());

            //descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<OTHER_MODULE_NAMEContext>));
            //services.Remove(descriptor!);
            //services.AddDbContext<OTHER_MODULE_NAMEContext>(SetupTestContext());

            return services;
        }
    }
}
