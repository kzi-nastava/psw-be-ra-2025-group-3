using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourSearchAndFilterQueryTests : BaseToursIntegrationTest
{
    public TourSearchAndFilterQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Filters_tours_by_name()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITouristTourService>();

        var filters = new TourFilterDto
        {
            Name = "Published"
        };

        // Act
        var result = service.SearchAndFilterTours(filters);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.ShouldAllBe(t => t.Name.Contains("Published"));
    }

    [Fact]
    public void Filters_tours_by_tags()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITouristTourService>();

        var filters = new TourFilterDto
        {
            Tags = new List<string> { "hiking", "adventure" }
        };

        // Act
        var result = service.SearchAndFilterTours(filters);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.ShouldAllBe(t => t.Tags.Any(tag => filters.Tags.Contains(tag)));
    }

    [Fact]
    public void Filters_tours_by_difficulty_range()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITouristTourService>();

        var filters = new TourFilterDto
        {
            Difficulties = new List<int> { 0, 1 } // Easy i Medium
        };

        // Act
        var result = service.SearchAndFilterTours(filters);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.ShouldAllBe(t => t.Difficulty == 0 || t.Difficulty == 1);
    }

    [Fact]
    public void Filters_tours_by_price_range()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITouristTourService>();

        var filters = new TourFilterDto
        {
            MinPrice = 50,
            MaxPrice = 150
        };

        // Act
        var result = service.SearchAndFilterTours(filters);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.ShouldAllBe(t => t.Price >= 50 && t.Price <= 150);
    }

    [Fact]
    public void Filters_tours_by_minimum_rating()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITouristTourService>();

        var filters = new TourFilterDto
        {
            MinRating = 4.0
        };

        // Act
        var result = service.SearchAndFilterTours(filters);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.ShouldAllBe(t => t.AverageRating >= 4.0);
    }

    [Fact]
    public void Filters_tours_by_combined_criteria()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITouristTourService>();

        var filters = new TourFilterDto
        {
            Name = "City",
            Tags = new List<string> { "culture" },
            MaxPrice = 100
        };

        // Act
        var result = service.SearchAndFilterTours(filters);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldAllBe(t => t.Name.Contains("City"));
        result.ShouldAllBe(t => t.Tags.Contains("culture"));
        result.ShouldAllBe(t => t.Price <= 100);
    }

    [Fact]
    public void Returns_empty_list_when_no_tours_match()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITouristTourService>();

        var filters = new TourFilterDto
        {
            Name = "NonexistentTour12345",
            MinPrice = 999999
        };

        // Act
        var result = service.SearchAndFilterTours(filters);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    public void Returns_all_published_tours_when_no_filters_applied()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITouristTourService>();

        var filters = new TourFilterDto(); // Prazan filter

        // Act
        var result = service.SearchAndFilterTours(filters);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
    }
}
