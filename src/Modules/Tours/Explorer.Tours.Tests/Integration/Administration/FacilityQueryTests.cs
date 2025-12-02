using Explorer.API.Controllers.Administration;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class FacilityQueryTests : BaseToursIntegrationTest
{
    public FacilityQueryTests(ToursTestFactory factory) : base(factory) { }

    private static FacilityController CreateController(IServiceScope scope)
    {
        return new FacilityController(scope.ServiceProvider.GetRequiredService<IFacilityService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }

    [Fact]
    public void Retrieves_all()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Kreiramo dva facility-ja da bismo očekivali rezultate
        controller.Create(new FacilityCreateDto
        {
            Name = "F1",
            Latitude = 10,
            Longitude = 10,
            Category = 0
        });

        controller.Create(new FacilityCreateDto
        {
            Name = "F2",
            Latitude = 20,
            Longitude = 20,
            Category = 1
        });

        var response = ((ObjectResult)controller.GetAll().Result)?.Value as List<FacilityDto>;

        response.ShouldNotBeNull();
        response!.Count.ShouldBeGreaterThanOrEqualTo(2);
    }
}
