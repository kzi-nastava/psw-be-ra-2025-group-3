using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Tests;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Explorer.Payments.Tests
{
    public class PaymentsTestFactory: BaseTestFactory<PaymentsContext>
    {
        protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PaymentsContext>));
            services.Remove(descriptor!);
            services.AddDbContext<PaymentsContext>(SetupTestContext());

            var existingInternalTour = services.FirstOrDefault(d => d.ServiceType == typeof(IInternalTourService));
            if (existingInternalTour != null) services.Remove(existingInternalTour);

            var internalTourMock = new Mock<IInternalTourService>();
            internalTourMock.Setup(s => s.GetById(-2)).Returns(new Explorer.Tours.API.Dtos.TourDto
            {
                Id = -2,
                Name = "Test Tour -2",
                Price = 500m,
                Status = (int)Explorer.Tours.API.Dtos.TourStatusDto.Published,
                ArchivedAt = null
            });
            internalTourMock.Setup(s => s.GetById(-4)).Returns(new TourDto
            {
                Id = -4,
                Name = "Test Tour -4",
                Price = 700m,
                Status = (int)TourStatusDto.Published,
                ArchivedAt = null
            });

            services.AddScoped<IInternalTourService>(_ => internalTourMock.Object);            

            return services;
        }
    }
}
