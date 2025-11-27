using Explorer.API.Controllers.Tourist.EquipmentInventory;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TouristEquipmentCommandTests : BaseToursIntegrationTest
{
    public TouristEquipmentCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Adds_equipment_to_tourist()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        long equipmentId = -3;

        var result = ((ObjectResult)controller.AddEquipment(equipmentId).Result)?.Value as EquipmentWithOwnershipDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(-3);
        result.Name.ShouldBe("Obična baterijska lampa");
        result.IsOwnedByTourist.ShouldBeTrue();

        var storedEntity = dbContext.TouristEquipment
            .FirstOrDefault(te => te.TouristId == -21 && te.EquipmentId == -3);
        storedEntity.ShouldNotBeNull();
    }

    [Fact]
    public void Add_fails_when_equipment_already_owned()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        long equipmentId = -1;

        Should.Throw<InvalidOperationException>(() => controller.AddEquipment(equipmentId));
    }

    [Fact]
    public void Deletes_equipment_from_tourist()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        long equipmentId = -2;

        var result = controller.DeleteEquipment(equipmentId);

        result.ShouldNotBeNull();
        ((NoContentResult)result).StatusCode.ShouldBe(204);

        var storedEntity = dbContext.TouristEquipment
            .FirstOrDefault(te => te.TouristId == -21 && te.EquipmentId == -2);
        storedEntity.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_when_equipment_not_owned()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        const long touristId = -21;
        const long equipmentId = -3;

        // Arrange: obezbedi da turist ne poseduje ovu opremu
        var existingRelations = dbContext.TouristEquipment
            .Where(te => te.TouristId == touristId && te.EquipmentId == equipmentId);

        dbContext.TouristEquipment.RemoveRange(existingRelations);
        dbContext.SaveChanges();

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.DeleteEquipment(equipmentId));
    }


    private static TouristEquipmentController CreateController(IServiceScope scope)
    {
        return new TouristEquipmentController(scope.ServiceProvider.GetRequiredService<ITouristEquipmentService>())
        {
            ControllerContext = BuildContext("-21")
        };
    }
}