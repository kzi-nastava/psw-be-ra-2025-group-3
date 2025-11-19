using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration.Administration;

[Collection("Sequential")]
public class AccountCommandTests : BaseStakeholdersIntegrationTest
{
    public AccountCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_account()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAccountService>();

        var dto = new AccountCreateDto
        {
            Username = "new_test_user",
            Password = "StrongPass123",
            Email = "new@test.com",
            Role = "Tourist"
        };

        // Act
        var result = service.Create(dto);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Username.ShouldBe(dto.Username);
        result.Email.ShouldBe(dto.Email);
        result.Role.ShouldBe(dto.Role);
        result.Status.ShouldBe("Active");
    }

    [Fact]
    public void Blocks_account()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAccountService>();

        // Act
        service.Block(-2);
        var result = service.Get(-2);

        // Assert
        result.Status.ShouldBe("Blocked");
    }

    [Fact]
    public void Unblocks_account()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAccountService>();

        // Act
        service.Unblock(-2);
        var result = service.Get(-2);

        // Assert
        result.Status.ShouldBe("Active");
    }

    [Fact]
    public void Fails_to_block_admin()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAccountService>();

        // Act & Assert
        Should.Throw<EntityValidationException>(() =>
        {
            service.Block(-1);
        });
    }

    [Fact]
    public void Fails_to_create_invalid_email()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAccountService>();

        var dto = new AccountCreateDto
        {
            Username = "bad_email_user",
            Password = "Valid123",
            Email = "invalid_email",
            Role = "Tourist"
        };

        // Act & Assert
        Should.Throw<EntityValidationException>(() =>
        {
            service.Create(dto);
        }).Message.ShouldContain("Invalid email");
    }
}
