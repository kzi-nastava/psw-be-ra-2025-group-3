using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourProblemCommandTests : BaseToursIntegrationTest
{
    public TourProblemCommandTests(ToursTestFactory factory) : base(factory)
    {
        // Resetuj bazu pre testova
        ResetDatabase();
    }

    private void ResetDatabase()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        dbContext.Database.ExecuteSqlRaw(@"
            DELETE FROM tours.""Messages"";
            DELETE FROM tours.""TourProblems"";
            
            INSERT INTO tours.""TourProblems""(
                ""Id"", ""TourId"", ""TouristId"", ""AuthorId"", ""Category"", ""Priority"", 
                ""Description"", ""Time"", ""CreatedAt"", ""UpdatedAt"", 
                ""Status"", ""ResolvedByTouristComment"", ""IsHighlighted"", ""AdminDeadline""
            )
            VALUES 
            (-1, -1, -11, -21, 0, 2, 'Bus was 30 minutes late at pickup point', '2024-11-15 08:30:00', '2024-11-15 09:00:00', NULL, 0, NULL, false, NULL),
            (-2, -1, -11, -21, 2, 1, 'Tour guide spoke too quietly, hard to hear explanations', '2024-11-15 10:15:00', '2024-11-15 12:00:00', NULL, 0, NULL, false, NULL),
            (-3, -2, -12, -22, 3, 3, 'Main attraction was closed without prior notice', '2024-11-14 14:00:00', '2024-11-14 15:30:00', NULL, 0, NULL, false, NULL),
            (-4, -1, -12, -21, 4, 0, 'Lunch portion was smaller than expected', '2024-11-15 12:30:00', '2024-11-15 13:00:00', NULL, 0, NULL, false, NULL);
        ");
    }

    [Fact]
    public void Creates_problem()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        var newProblem = new TourProblemCreateDto
        {
            TourId = -1,
            Category = 0,
            Priority = 2,
            Description = "Test problem description with more than 10 characters",
            Time = DateTime.UtcNow.AddHours(-2)
        };

        // Act
        var result = service.Create(newProblem, -11);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.TourId.ShouldBe(newProblem.TourId);
        result.TouristId.ShouldBe(-11);
        result.Category.ShouldBe(newProblem.Category);
        result.Priority.ShouldBe(newProblem.Priority);
        result.Description.ShouldBe(newProblem.Description);
    }

    [Fact]
    public void Create_fails_invalid_description()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        var newProblem = new TourProblemCreateDto
        {
            TourId = -1,
            Category = 0,
            Priority = 2,
            Description = "Short",
            Time = DateTime.UtcNow.AddHours(-2)
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => service.Create(newProblem, -11));
    }

    [Fact]
    public void Updates_problem()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        var updateDto = new TourProblemUpdateDto
        {
            Id = -1,
            Category = 1,
            Priority = 3,
            Description = "Updated description with sufficient length for validation",
            Time = DateTime.UtcNow.AddHours(-1)
        };

        // Act
        var result = service.Update(updateDto, -11);

        // Assert
        result.ShouldNotBeNull();
        result.Category.ShouldBe(1);
        result.Priority.ShouldBe(3);
        result.Description.ShouldBe(updateDto.Description);
    }

    [Fact]
    public void Update_fails_invalid_user()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        var updateDto = new TourProblemUpdateDto
        {
            Id = -1,
            Category = 1,
            Priority = 3,
            Description = "Updated description with sufficient length",
            Time = DateTime.UtcNow.AddHours(-1)
        };

        // Act & Assert
        Should.Throw<Exception>(() => service.Update(updateDto, -12));
    }

    [Fact]
    public void Deletes_problem()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        service.Delete(-2, -11);

        // Assert
        var deletedProblem = dbContext.TourProblems.Find(-2L);
        deletedProblem.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_user()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        // Act & Assert
        Should.Throw<Exception>(() => service.Delete(-1, -12));
    }
}