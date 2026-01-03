using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration.Wallet;

[Collection("Sequential")]
public class WalletQueryTests : BaseStakeholdersIntegrationTest
{
    public WalletQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    private static ClaimsPrincipal BuildUser(long personId)
    {
        var claims = new[] { new Claim("personId", personId.ToString()) };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }

    private static ControllerContext BuildContext(long personId)
        => new ControllerContext { HttpContext = new DefaultHttpContext { User = BuildUser(personId) } };

    private static TouristWalletController CreateController(IServiceScope scope, long personId)
    {
        var service = scope.ServiceProvider.GetRequiredService<IWalletService>();
        return new TouristWalletController(service) { ControllerContext = BuildContext(personId) };
    }

    [Fact]
    public void Tourist_can_get_my_wallet()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -21);

        var actionResult = controller.GetMyWallet();
        var ok = actionResult.Result as OkObjectResult;

        ok.ShouldNotBeNull();
        var dto = ok!.Value as WalletDto;

        dto.ShouldNotBeNull();
        dto!.PersonId.ShouldBe(-21);
        dto.BalanceAc.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void Author_cannot_get_my_wallet()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -11); // autor

        Should.Throw<UnauthorizedAccessException>(() => controller.GetMyWallet());
    }

    [Fact]
    public void Admin_cannot_get_my_wallet()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -1); // admin

        Should.Throw<UnauthorizedAccessException>(() => controller.GetMyWallet());
    }
}
