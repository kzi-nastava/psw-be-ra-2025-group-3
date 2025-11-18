using Explorer.Stakeholders.API.Public;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration.Administration;

[Collection("Sequential")]
public class AccountQueryTests : BaseStakeholdersIntegrationTest
{
    public AccountQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Gets_account_by_id()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAccountService>();

        var result = service.Get(-1);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Username.ShouldBe("test_admin"); // promenjeno
        result.Email.ShouldBe("admin.test@explorer.com"); // promenjeno
        result.Role.ShouldBe("Admin"); // ako mapira 0 -> Admin
        result.Status.ShouldBe("Active");
    }

    [Fact]
    public void Gets_all_accounts()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAccountService>();

        // Act
        var result = service.GetAll();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
        result.Any(a => a.Id == -1).ShouldBeTrue();
    }

    [Fact]
    public void Fails_to_get_nonexistent_account()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAccountService>();

        // Act & Assert
        Should.Throw<KeyNotFoundException>(() => service.Get(-999));
    }
}
