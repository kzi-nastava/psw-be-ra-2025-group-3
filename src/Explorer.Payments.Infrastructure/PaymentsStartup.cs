using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.Core.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Explorer.Payments.Infrastructure.Database;
using Explorer.BuildingBlocks.Infrastructure.Database;

namespace Explorer.Payments.Infrastructure
{
    public static class PaymentsStartup
    {
        public static IServiceCollection ConfigurePaymentsModule(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(PaymentProfile).Assembly); // Preduslov da imamo ovu liniju koda je da smo definisali već Profile klasu u Core/Mappers
            SetupCore(services);
            SetupInfrastructure(services);
            return services;
        }

        private static void SetupCore(IServiceCollection services)
        {
        }

        private static void SetupInfrastructure(IServiceCollection services)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("payments"));
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<PaymentsContext>(opt =>
                opt.UseNpgsql(dataSource,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "payments")));
        }

    }
}
