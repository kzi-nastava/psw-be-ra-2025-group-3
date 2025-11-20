using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using Explorer.Blog.API.Dtos;
using Explorer.BuildingBlocks.Tests;
using Xunit;

namespace Explorer.Blog.Tests.Integration;

public class FacilityControllerTests : BaseWebIntegrationTest<BlogTestFactory>
{
    private readonly HttpClient _client;

    public FacilityControllerTests(BlogTestFactory factory) : base(factory)
    {
        _client = factory.CreateClient();

        
        _client.DefaultRequestHeaders.Add("personId", "-1");
    }

    [Fact]
    public async Task GetAll_ReturnsSeededFacilities()
    {
        var response = await _client.GetAsync("/api/administration/facilities");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var items = await response.Content.ReadFromJsonAsync<List<FacilityDto>>();

        Assert.NotNull(items);
        Assert.Contains(items!, f => f.Id == -1);
    }

    [Fact]
    public async Task Create_ReturnsNewFacility()
    {
        var dto = new FacilityCreateDto
        {
            Name = "Integration Facility",
            Latitude = 40.0,
            Longitude = 50.0,
            Category = 1
        };

        var response = await _client.PostAsJsonAsync("/api/administration/facilities", dto);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<FacilityDto>();

        Assert.NotNull(created);
        Assert.Equal("Integration Facility", created!.Name);
    }

    [Fact]
    public async Task Update_ExistingFacility_Works()
    {
        var dto = new FacilityUpdateDto
        {
            Name = "Updated Name",
            Latitude = 11.123,
            Longitude = 22.456,
            Category = 2
        };

        var response = await _client.PutAsJsonAsync("/api/administration/facilities/-1", dto);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await response.Content.ReadFromJsonAsync<FacilityDto>();

        Assert.NotNull(updated);
        Assert.Equal("Updated Name", updated!.Name);
        Assert.Equal(2, updated.Category);
    }

    [Fact]
    public async Task Update_NonExistingFacility_ReturnsNotFound()
    {
        var dto = new FacilityUpdateDto
        {
            Name = "DoesNotExist",
            Latitude = 1,
            Longitude = 1,
            Category = 1
        };

        var response = await _client.PutAsJsonAsync("/api/administration/facilities/-999", dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingFacility_Works()
    {
        var response = await _client.DeleteAsync("/api/administration/facilities/-1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NonExistingFacility_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/administration/facilities/-999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
