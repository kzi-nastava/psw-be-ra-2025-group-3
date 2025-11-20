using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class MonumentCommandTests : BaseToursIntegrationTest
{
    public MonumentCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_monument()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newEntity = new MonumentDto
        {
            Name = "Test Spomenik",
            Description = "Opis test spomenika.",
            Year = 2000,
            Status = "Active",   
            Latitude = 45.251,
            Longitude = 19.836
        };

        // Act
        var actionResult = controller.Create(newEntity).Result as ObjectResult;
        actionResult.ShouldNotBeNull();
        actionResult.StatusCode.ShouldBe(200);

        var result = actionResult.Value as MonumentDto;

        // Assert – Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.Description.ShouldBe(newEntity.Description);
        result.Year.ShouldBe(newEntity.Year);
        result.Status.ShouldBe(newEntity.Status);
        result.Latitude.ShouldBe(newEntity.Latitude);
        result.Longitude.ShouldBe(newEntity.Longitude);

        // Assert – Database
        var storedEntity = dbContext.Monuments.FirstOrDefault(i => i.Name == newEntity.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidEntity = new MonumentDto
        {
            Description = "Test"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidEntity));
    }

    [Fact]
    public void Updates_monument()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedEntity = new MonumentDto
        {
            Id = -1,
            Name = "Izmenjeni Spomenik",
            Description = "Izmenjeni opis.",
            Year = 1900,
            Status = "Inactive",
            Latitude = 45.200,
            Longitude = 19.800
        };

        // Act
        var actionResult = controller.Update(updatedEntity).Result as ObjectResult;
        actionResult.ShouldNotBeNull();
        actionResult.StatusCode.ShouldBe(200);

        var result = actionResult.Value as MonumentDto;

        // Assert – Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe(updatedEntity.Name);
        result.Description.ShouldBe(updatedEntity.Description);
        result.Year.ShouldBe(updatedEntity.Year);
        result.Status.ShouldBe(updatedEntity.Status);
        result.Latitude.ShouldBe(updatedEntity.Latitude);
        result.Longitude.ShouldBe(updatedEntity.Longitude);

        // Assert – Database
        var storedEntity = dbContext.Monuments.FirstOrDefault(i => i.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Name.ShouldBe(updatedEntity.Name);
        storedEntity.Description.ShouldBe(updatedEntity.Description);
        storedEntity.Year.ShouldBe(updatedEntity.Year);
        storedEntity.Status.ToString().ShouldBe(updatedEntity.Status);
        storedEntity.Latitude.ShouldBe(updatedEntity.Latitude);
        storedEntity.Longitude.ShouldBe(updatedEntity.Longitude);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var updatedEntity = new MonumentDto
        {
            Id = -9999,
            Name = "Test",
            Description = "Description",
            Year = 2000,
            Status = "Active",
            Latitude = 45.0,
            Longitude = 19.0
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(updatedEntity));
    }

    [Fact]
    public void Deletes_monument()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = (OkResult)controller.Delete(-3);

        // Assert – Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert – Database
        var stored = dbContext.Monuments.FirstOrDefault(i => i.Id == -3);
        stored.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Delete(-9999));
    }

    private static MonumentController CreateController(IServiceScope scope)
    {
        return new MonumentController(scope.ServiceProvider.GetRequiredService<IMonumentService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
