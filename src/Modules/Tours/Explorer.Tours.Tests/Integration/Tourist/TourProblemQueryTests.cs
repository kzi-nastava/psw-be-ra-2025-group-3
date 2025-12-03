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

    [Theory]
    [InlineData(-21, 3)] // FIXED: Author -21 has 3 problems on his tours (-1, -2, and -4)
    [InlineData(-22, 1)] // Author -22 has 1 problem on his tours (-3)
    public void Retrieves_problems_by_author_id(long authorId, int expectedCount)
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        // Act
        var result = service.GetByAuthorId(authorId);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(expectedCount);
        result.ShouldAllBe(p => p.AuthorId == authorId);
    }

    [Fact]
    public void Get_by_id_succeeds_for_author()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        // Act - Author -21 can view problem -1 on his tour
        var result = service.GetById(-1, -21);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.AuthorId.ShouldBe(-21);
    }

    [Fact]
    public void Get_by_id_fails_for_unauthorized_user()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        // Act & Assert - User -22 cannot view problem -1 (not tourist who reported it, not author)
        Should.Throw<Exception>(() => service.GetById(-1, -22));
    }
}