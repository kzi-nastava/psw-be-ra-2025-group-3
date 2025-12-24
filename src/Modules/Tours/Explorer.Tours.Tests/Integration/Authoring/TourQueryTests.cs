using System;
using System.Linq;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class TourQueryTests : BaseToursIntegrationTest
{
    public TourQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Gets_tour_by_id()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();

        var created = service.Create(new TourCreateDto
        {
            Name = "Test Tour",
            Description = "Desc",
            Difficulty = 0,
            Tags = new List<string>()
        }, -11);

        // ACT
        var result = service.GetById(created.Id);

        // ASSERT
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Name.ShouldBe("Test Tour");
        result.AuthorId.ShouldBe(-11);
    }


    [Fact]
    public void Gets_tours_by_author_id()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();

        service.Create(new TourCreateDto
        {
            Name = "T1",
            Description = "D",
            Difficulty = 0,
            Tags = new List<string>()
        }, -11);

        service.Create(new TourCreateDto
        {
            Name = "T2",
            Description = "D",
            Difficulty = 1,
            Tags = new List<string>()
        }, -11);

        // ACT
        var result = service.GetByAuthorId(-11);

        // ASSERT
        result.ShouldNotBeNull();
        result.ShouldContain(t => t.Name == "T1");
        result.ShouldContain(t => t.Name == "T2");
        result.All(t => t.AuthorId == -11).ShouldBeTrue();
    }


    [Fact]
    public void Fails_to_get_nonexistent_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();

        // Act & Assert
        Should.Throw<Exception>(() => service.GetById(-999));
    }
}