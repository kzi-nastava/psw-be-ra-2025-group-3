using Explorer.Tours.API.Public.Authoring;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class TourQueryTests : BaseToursIntegrationTest
{
    public TourQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Gets_tour_by_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();

        // Act
        var result = service.GetById(-1);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe("Test Tour Draft");
        result.AuthorId.ShouldBe(-11); 
    }

    [Fact]
    public void Gets_tours_by_author_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();

        // Act
        var result = service.GetByAuthorId(-11);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3); // 3 test tours
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