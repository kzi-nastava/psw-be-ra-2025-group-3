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
public class EquipmentCommandTests : BaseToursIntegrationTest
{
    public EquipmentCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newEntity = new EquipmentDto
        {
            Name = "Obuća za grub teren",
            Description = "Patike sa tvrdim đonom i kramponima koje daju stabilnost."
        };

        // ACT
        var result =
            ((ObjectResult)controller.Create(newEntity).Result)?.Value as EquipmentDto;

        // ASSERT – response
        result.ShouldNotBeNull();
        result.Id.ShouldBeGreaterThan(0);
        result.Name.ShouldBe(newEntity.Name);

        // ASSERT – database
        var storedEntity = dbContext.Equipment
            .FirstOrDefault(e => e.Id == result.Id);

        storedEntity.ShouldNotBeNull();
        storedEntity.Name.ShouldBe(newEntity.Name);
    }


    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new EquipmentDto
        {
            Description = "Test"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(updatedEntity));
    }

    [Fact]
    public void Updates_without_id_does_not_change_entity()
    {
        // ---------- CREATE ----------
        using (var scope = Factory.Services.CreateScope())
        {
            var controller = CreateController(scope);

            controller.Create(new EquipmentDto
            {
                Name = "Voda",
                Description = "Stara"
            });
        }

        // ---------- UPDATE ----------
        using (var scope = Factory.Services.CreateScope())
        {
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var updatedEntity = new EquipmentDto
            {
                Name = "Voda",
                Description = "Nova"
            };

            // ACT
            controller.Update(updatedEntity);

            // ASSERT – entity is NOT changed
            var stored = dbContext.Equipment.FirstOrDefault(e => e.Name == "Voda");
            stored.ShouldNotBeNull();
            stored.Description.ShouldBe("Stara"); // 👈 očekivano
        }
    }




    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new EquipmentDto
        {
            Id = -1000,
            Name = "Test"
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // ARRANGE – create
        var createDto = new EquipmentDto
        {
            Name = "Delete Equipment",
            Description = "To be deleted"
        };

        var created =
            ((ObjectResult)controller.Create(createDto).Result)?.Value as EquipmentDto;

        created.ShouldNotBeNull();
        var id = created.Id;

        // ACT
        var result = controller.Delete(id) as OkResult;

        // ASSERT
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        dbContext.Equipment.FirstOrDefault(e => e.Id == id).ShouldBeNull();
    }


    [Fact]
    public void Delete_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Delete(-1000));
    }
    
    private static EquipmentController CreateController(IServiceScope scope)
    {
        return new EquipmentController(scope.ServiceProvider.GetRequiredService<IEquipmentService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}