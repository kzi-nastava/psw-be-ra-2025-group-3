using Explorer.API.Controllers.Tourist.EquipmentInventory;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TouristEquipmentQueryTests : BaseToursIntegrationTest
{
    public TouristEquipmentQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all_equipment_with_ownership()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetAllEquipmentWithOwnership().Result)?.Value as List<EquipmentWithOwnershipDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);

        var ownedEquipment = result.Where(e => e.IsOwnedByTourist).ToList();
        ownedEquipment.Count.ShouldBe(2);
        ownedEquipment.ShouldContain(e => e.Id == -1 && e.Name == "Voda");
        ownedEquipment.ShouldContain(e => e.Id == -2 && e.Name == "Štapovi za šetanje");

        var notOwnedEquipment = result.Where(e => !e.IsOwnedByTourist).ToList();
        notOwnedEquipment.Count.ShouldBe(1);
        notOwnedEquipment.ShouldContain(e => e.Id == -3 && e.Name == "Obična baterijska lampa");
    }

    [Fact]
    public void Retrieves_only_tourist_equipment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetMyEquipment().Result)?.Value as List<EquipmentWithOwnershipDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.ShouldAllBe(e => e.IsOwnedByTourist == true);
        result.ShouldContain(e => e.Id == -1 && e.Name == "Voda");
        result.ShouldContain(e => e.Id == -2 && e.Name == "Štapovi za šetanje");
    }

    private static TouristEquipmentController CreateController(IServiceScope scope)
    {
        return new TouristEquipmentController(scope.ServiceProvider.GetRequiredService<ITouristEquipmentService>())
        {
            ControllerContext = BuildContext("-21")  // turista1@gmail.com
        };
    }
}