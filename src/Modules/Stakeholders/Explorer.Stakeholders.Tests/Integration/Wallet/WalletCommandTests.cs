using Explorer.API.Controllers;
using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration.Wallet;

[Collection("Sequential")]
public class WalletCommandTests : BaseStakeholdersIntegrationTest
{
    public WalletCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    private static ClaimsPrincipal BuildUser(long personId)
    {
        var claims = new[] { new Claim("personId", personId.ToString()) };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }

    private static ControllerContext BuildContext(long personId)
        => new ControllerContext { HttpContext = new DefaultHttpContext { User = BuildUser(personId) } };

    private static AdminWalletController CreateAdminController(IServiceScope scope, long adminPersonId)
    {
        var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        return new AdminWalletController(walletService, notificationService)
        {
            ControllerContext = BuildContext(adminPersonId)
        };
    }

    [Fact]
    public void Admin_can_topup_tourist_wallet()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdminController(scope, -1);

        var request = new WalletTopUpDto { TouristUserId = -21, AmountAc = 100 };

        var actionResult = controller.TopUp(request);
        var ok = actionResult.Result as OkObjectResult;

        ok.ShouldNotBeNull();

        var dto = ok!.Value as WalletDto;
        dto.ShouldNotBeNull();
        dto!.PersonId.ShouldBe(-21);
        dto.BalanceAc.ShouldBeGreaterThanOrEqualTo(100);
    }

    [Fact]
    public void Topup_with_amount_zero_throws()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdminController(scope, -1);

        var request = new WalletTopUpDto { TouristUserId = -21, AmountAc = 0 };

        Should.Throw<ArgumentException>(() => controller.TopUp(request));
    }

    [Fact]
    public void Topup_non_existing_tourist_throws_key_not_found()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdminController(scope, -1);

        var request = new WalletTopUpDto { TouristUserId = -999, AmountAc = 10 };

        Should.Throw<KeyNotFoundException>(() => controller.TopUp(request));
    }

    [Fact]
    public void Topup_for_author_user_throws_unauthorized()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdminController(scope, -1);

        var request = new WalletTopUpDto { TouristUserId = -11, AmountAc = 10 }; // autor

        Should.Throw<UnauthorizedAccessException>(() => controller.TopUp(request));
    }

    [Fact]
    public void Non_admin_cannot_topup()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdminController(scope, -21); // turist kao "admin"

        var request = new WalletTopUpDto { TouristUserId = -22, AmountAc = 10 };

        Should.Throw<UnauthorizedAccessException>(() => controller.TopUp(request));
    }

    [Fact]
    public void Multiple_topups_are_accumulated()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdminController(scope, -1);

        var first = new WalletTopUpDto { TouristUserId = -22, AmountAc = 40 };
        var second = new WalletTopUpDto { TouristUserId = -22, AmountAc = 60 };

        var ok1 = controller.TopUp(first).Result.ShouldBeOfType<OkObjectResult>();
        var dto1 = ok1.Value.ShouldBeOfType<WalletDto>();

        var ok2 = controller.TopUp(second).Result.ShouldBeOfType<OkObjectResult>();
        var dto2 = ok2.Value.ShouldBeOfType<WalletDto>();

        dto2.PersonId.ShouldBe(-22);
        dto2.BalanceAc.ShouldBe(dto1.BalanceAc + 60);
    }

    [Fact]
    public void Topup_creates_notification_for_tourist()
    {
        using var scope = Factory.Services.CreateScope();

        var walletController = CreateAdminController(scope, -1);

        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var notificationController = new NotificationController(notificationService)
        {
            ControllerContext = BuildContext(-21) // turist
        };

        var request = new WalletTopUpDto { TouristUserId = -21, AmountAc = 25 };

        walletController.TopUp(request);

        var actionResult = notificationController.GetMyNotifications();
        var ok = actionResult.Result as OkObjectResult;

        ok.ShouldNotBeNull();
        var list = ok!.Value.ShouldBeAssignableTo<List<NotificationDto>>();

        list.Any(n => n.Type == (int)NotificationType.WalletTopUp
                    && n.RecipientId == -21)
            .ShouldBeTrue();
    }

}