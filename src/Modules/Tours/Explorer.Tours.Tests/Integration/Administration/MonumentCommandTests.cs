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
        dbContext.Monuments.RemoveRange(dbContext.Monuments);
        dbContext.SaveChanges();
        var newEntity = new MonumentDto
        {
            Name = "Test Spomenik",
            Description = "Opis test spomenika.",
            Year = 2000,
            Status = 1,   
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
        long id;

        // ---------- CREATE ----------
        using (var scope = Factory.Services.CreateScope())
        {
            var controller = CreateController(scope);

            var createdResult = controller.Create(new MonumentDto
            {
                Name = "Spomenik za update",
                Description = "Stari opis",
                Year = 2000,
                Status = 1,
                Latitude = 45.0,
                Longitude = 19.0
            }).Result as ObjectResult;

            var created = createdResult!.Value as MonumentDto;
            id = created!.Id;
        }

        // ---------- UPDATE + ASSERT ----------
        using (var scope = Factory.Services.CreateScope())
        {
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var updatedEntity = new MonumentDto
            {
                Id = id,
                Name = "Izmenjeni Spomenik",
                Description = "Izmenjeni opis",
                Year = 1999,
                Status = 0,
                Latitude = 45.2,
                Longitude = 19.8
            };

            var actionResult = controller.Update(id, updatedEntity).Result as ObjectResult;
            actionResult.ShouldNotBeNull();

            var stored = dbContext.Monuments.FirstOrDefault(m => m.Id == id);
            stored.ShouldNotBeNull();
            stored!.Name.ShouldBe("Izmenjeni Spomenik");
            stored.Description.ShouldBe("Izmenjeni opis");
        }
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
            Status = 1,
            Latitude = 45.0,
            Longitude = 19.0
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(updatedEntity.Id, updatedEntity));
    }

    [Fact]
    public void Deletes_monument()
    {
        long id;

        // ---------- CREATE ----------
        using (var scope = Factory.Services.CreateScope())
        {
            var controller = CreateController(scope);

            var createdResult = controller.Create(new MonumentDto
            {
                Name = "Spomenik za brisanje",
                Description = "Opis",
                Year = 2000,
                Status = 1,
                Latitude = 45,
                Longitude = 19
            }).Result as ObjectResult;

            var created = createdResult!.Value as MonumentDto;
            id = created!.Id;
        }

        // ---------- DELETE + ASSERT ----------
        using (var scope = Factory.Services.CreateScope())
        {
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var result = controller.Delete(id) as OkResult;
            result.ShouldNotBeNull();

            dbContext.Monuments.FirstOrDefault(m => m.Id == id)
                .ShouldBeNull();
        }
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
