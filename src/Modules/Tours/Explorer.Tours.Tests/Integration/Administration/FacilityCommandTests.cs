using Explorer.API.Controllers.Administration;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class FacilityCommandTests : BaseToursIntegrationTest
{
    public FacilityCommandTests(ToursTestFactory factory) : base(factory) { }

    private static FacilityController CreateController(IServiceScope scope)
    {
        return new FacilityController(scope.ServiceProvider.GetRequiredService<IFacilityService>())
        {
            ControllerContext = BuildContext("-1") // admin
        };
    }

    [Fact]
    public void Creates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var dto = new FacilityCreateDto
        {
            Name = "Test Facility",
            Latitude = 45.25,
            Longitude = 19.83,
            Category = 1
        };

        var result = ((ObjectResult)controller.Create(dto).Result)?.Value as FacilityDto;

        result.ShouldNotBeNull();
        result!.Id.ShouldNotBe(0);

        db.Facilities.Any(f => f.Id == result.Id).ShouldBeTrue();
    }

    [Fact]
    public void Updates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // prvo kreiramo facility da bismo imali ID
        var created = ((ObjectResult)controller.Create(new FacilityCreateDto
        {
            Name = "Initial",
            Latitude = 10,
            Longitude = 10,
            Category = 0
        }).Result)?.Value as FacilityDto;

        created.ShouldNotBeNull();

        var updateDto = new FacilityUpdateDto
        {
            Name = "Updated Facility",
            Latitude = 50,
            Longitude = 50,
            Category = 2
        };

        var updated = ((ObjectResult)controller.Update(created!.Id, updateDto).Result)?.Value as FacilityDto;

        updated.ShouldNotBeNull();
        updated!.Name.ShouldBe("Updated Facility");

        var stored = db.Facilities.FirstOrDefault(f => f.Id == created.Id);
        stored.ShouldNotBeNull();
        stored!.Name.ShouldBe("Updated Facility");
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new FacilityUpdateDto
        {
            Name = "X",
            Latitude = 0,
            Longitude = 0,
            Category = 1
        };

        Should.Throw<NotFoundException>(() => controller.Update(long.MaxValue, dto));
    }

    [Fact]
    public void Deletes()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // prvo kreiramo pa onda brišemo
        var created = ((ObjectResult)controller.Create(new FacilityCreateDto
        {
            Name = "ToDelete",
            Latitude = 10,
            Longitude = 10,
            Category = 0
        }).Result)?.Value as FacilityDto;

        created.ShouldNotBeNull();

        var result = (OkResult)controller.Delete(created!.Id);
        result.StatusCode.ShouldBe(200);

        db.Facilities.Any(f => f.Id == created.Id).ShouldBeFalse();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<NotFoundException>(() => controller.Delete(long.MaxValue));
    }
}
