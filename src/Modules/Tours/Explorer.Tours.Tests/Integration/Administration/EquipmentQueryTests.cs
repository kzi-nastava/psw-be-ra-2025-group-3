using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class EquipmentQueryTests : BaseToursIntegrationTest
{
    public EquipmentQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // ARRANGE – create test-specific data
        controller.Create(new EquipmentDto { Name = "EQ-A", Description = "D1" });
        controller.Create(new EquipmentDto { Name = "EQ-B", Description = "D2" });
        controller.Create(new EquipmentDto { Name = "EQ-C", Description = "D3" });

        // ACT
        var result =
            ((ObjectResult)controller.GetAll(0, 0).Result)?.Value
            as PagedResult<EquipmentDto>;

        // ASSERT – NE ZANIMA NAS UKUPAN BROJ
        result.ShouldNotBeNull();

        result.Results.ShouldContain(e => e.Name == "EQ-A");
        result.Results.ShouldContain(e => e.Name == "EQ-B");
        result.Results.ShouldContain(e => e.Name == "EQ-C");
    }



    private static EquipmentController CreateController(IServiceScope scope)
    {
        return new EquipmentController(scope.ServiceProvider.GetRequiredService<IEquipmentService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}