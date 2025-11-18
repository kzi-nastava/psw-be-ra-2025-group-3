using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourProblemQueryTests : BaseToursIntegrationTest
{
    public TourProblemQueryTests(ToursTestFactory factory) : base(factory)
    {
        // Resetuj bazu pre testova
        ResetDatabase();
    }

    private void ResetDatabase()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        dbContext.Database.ExecuteSqlRaw(@"
            DELETE FROM tours.""TourProblems"";
            
            INSERT INTO tours.""TourProblems""(""Id"", ""TourId"", ""TouristId"", ""Category"", ""Priority"", ""Description"", ""Time"", ""CreatedAt"", ""UpdatedAt"")
            VALUES 
            (-1, -1, -11, 0, 2, 'Bus was 30 minutes late at pickup point', '2024-11-15 08:30:00', '2024-11-15 09:00:00', NULL),
            (-2, -1, -11, 2, 1, 'Tour guide spoke too quietly, hard to hear explanations', '2024-11-15 10:15:00', '2024-11-15 12:00:00', NULL),
            (-3, -2, -12, 3, 3, 'Main attraction was closed without prior notice', '2024-11-14 14:00:00', '2024-11-14 15:30:00', NULL),
            (-4, -1, -12, 4, 0, 'Lunch portion was smaller than expected', '2024-11-15 12:30:00', '2024-11-15 13:00:00', NULL);
        ");
    }

    [Fact]
    public void Retrieves_all_tourist_problems()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        // Act
        var result = service.GetByTouristId(-11);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
    }

    [Fact]
    public void Retrieves_problem_by_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        // Act
        var result = service.GetById(-1, -11);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.TouristId.ShouldBe(-11);
        result.Category.ShouldBe(0);
        result.Priority.ShouldBe(2);
    }

    [Fact]
    public void Get_fails_invalid_user()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        // Act & Assert
        Should.Throw<Exception>(() => service.GetById(-1, -12));
    }
}