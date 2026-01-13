
using Explorer.BuildingBlocks.Tests;
using Explorer.Payments.API.Internal;
using Explorer.Payments.Infrastructure;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Internal;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;

namespace Explorer.Tours.Tests;

public class ToursTestFactory : BaseTestFactory<ToursContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        services.ConfigurePaymentsModule();

        // Tours DB Context
        var descriptor = services.SingleOrDefault(d =>
            d.ServiceType == typeof(DbContextOptions<ToursContext>));
        if (descriptor != null)
            services.Remove(descriptor);
        services.AddDbContext<ToursContext>(SetupTestContext());

        // Payments DB Context
        var paymentsDescriptor = services.SingleOrDefault(d =>
            d.ServiceType == typeof(DbContextOptions<PaymentsContext>));
        if (paymentsDescriptor != null)
            services.Remove(paymentsDescriptor);
        services.AddDbContext<PaymentsContext>(SetupTestContext());

        // ==================== MOCK: IInternalShoppingCartService ====================
        var existingShoppingCart = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IInternalShoppingCartService));
        if (existingShoppingCart != null)
            services.Remove(existingShoppingCart);

        var shoppingCartMock = new Mock<IInternalShoppingCartService>();
        shoppingCartMock.Setup(s => s.HasPurchasedTour(It.IsAny<long>(), It.IsAny<long>()))
                        .Returns(true);

        services.AddScoped<IInternalShoppingCartService>(_ => shoppingCartMock.Object);

        // ==================== MOCK: IInternalWalletService ====================
        var existingWallet = services.FirstOrDefault(d => d.ServiceType == typeof(IInternalWalletService));
        if (existingWallet != null) services.Remove(existingWallet);

        var walletMock = new Mock<IInternalWalletService>();
        walletMock.Setup(s => s.GetWallet(It.IsAny<long>()))
            .Returns((long personId) => new WalletDto
            {
                PersonId = personId,
                BalanceAc = 100000
            });
        walletMock.Setup(s => s.DeductAc(It.IsAny<long>(), It.IsAny<decimal>()))
            .Returns((long personId, decimal amount) => new WalletDto
            {
                PersonId = personId,
                BalanceAc = 100000 - (int)amount
            });

        services.AddScoped<IInternalWalletService>(_ => walletMock.Object);

        // ==================== MOCK: IInternalNotificationService ====================
        var existingNotification = services.FirstOrDefault(d => d.ServiceType == typeof(IInternalNotificationService));
        if (existingNotification != null) services.Remove(existingNotification);

        var notificationMock = new Mock<IInternalNotificationService>();
        notificationMock.Setup(s => s.CreateTourPurchaseNotification(
            It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>()));

        services.AddScoped<IInternalNotificationService>(_ => notificationMock.Object);

        // ==================== MOCK: IInternalTourService ====================
        var existingTour = services.FirstOrDefault(d => d.ServiceType == typeof(IInternalTourService));
        if (existingTour != null) services.Remove(existingTour);

        var tourMock = new Mock<IInternalTourService>();

        // Tour -2: OK
        tourMock.Setup(s => s.GetById(-2)).Returns(new TourDto
        {
            Id = -2,
            Name = "Test Tour -2",
            Price = 500m,
            Status = (int)TourStatusDto.Published,
            ArchivedAt = null
        });

        // Tour -3: Archived
        tourMock.Setup(s => s.GetById(-3)).Returns(new TourDto
        {
            Id = -3,
            Name = "Archived Tour",
            Price = 300m,
            Status = (int)TourStatusDto.Published,
            ArchivedAt = DateTime.UtcNow.AddDays(-10)
        });

        // Tour -1: Draft
        tourMock.Setup(s => s.GetById(-1)).Returns(new TourDto
        {
            Id = -1,
            Name = "Draft Tour",
            Price = 200m,
            Status = (int)TourStatusDto.Draft,
            ArchivedAt = null
        });

        // Nepoznati negativni ID-evi: null
        tourMock.Setup(s => s.GetById(It.Is<long>(id => id < 0 && id != -2 && id != -3 && id != -1)))
            .Returns((TourDto)null);

        // Pozitivni ID-evi: dinamički kreirani
        tourMock.Setup(s => s.GetById(It.Is<long>(id => id > 0)))
            .Returns((long tourId) => new TourDto
            {
                Id = tourId,
                Name = $"Test Tour {tourId}",
                Price = 500m,
                Status = (int)TourStatusDto.Published,
                ArchivedAt = null
            });

        services.AddScoped<IInternalTourService>(_ => tourMock.Object);

        return services;
    }
}