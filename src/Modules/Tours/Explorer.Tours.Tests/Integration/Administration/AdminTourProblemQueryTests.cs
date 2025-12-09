using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class AdminTourProblemQueryTests : BaseToursIntegrationTest
{
    public AdminTourProblemQueryTests(ToursTestFactory factory) : base(factory)
    {
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
            -- OVERDUE problems (older than 5 days)
            (-101, -1, -21, -11, 0, 3, 'Bus never arrived at pickup location', '2025-11-29 08:00:00', '2025-11-29 08:30:00', NULL, 0, NULL, false, NULL),
            (-102, -1, -21, -11, 1, 2, 'Hotel room was not available', '2025-12-02 18:00:00', '2025-12-02 18:30:00', NULL, 0, NULL, false, NULL),
            (-103, -2, -22, -12, 2, 1, 'Tour guide did not show up', '2025-12-03 09:00:00', '2025-12-03 09:30:00', NULL, 0, NULL, false, NULL),
            (-104, -1, -22, -11, 3, 3, 'Main attraction was closed', '2025-11-19 14:00:00', '2025-11-19 14:30:00', NULL, 0, NULL, false, NULL),
            
            -- FRESH problems (younger than 5 days)
            (-105, -1, -21, -11, 4, 1, 'Lunch portion was smaller', '2025-12-07 12:30:00', '2025-12-07 13:00:00', NULL, 0, NULL, false, NULL),
            (-106, -2, -22, -12, 0, 2, 'Bus had no air conditioning', '2025-12-08 10:00:00', '2025-12-08 10:30:00', NULL, 0, NULL, false, NULL),
            
            -- RESOLVED problem
            (-108, -1, -21, -11, 1, 2, 'Hotel changed our room', '2025-12-01 20:00:00', '2025-12-01 20:30:00', '2025-12-03 10:00:00', 1, 'Thank you!', false, NULL),
            
            -- UNRESOLVED problem
            (-110, -1, -21, -11, 3, 3, 'Promised refund never received', '2025-11-20 14:00:00', '2025-11-20 14:30:00', '2025-11-28 12:00:00', 2, 'Still waiting', false, NULL),
            
            -- Problem with messages
            (-111, -1, -21, -11, 0, 2, 'Bus driver was rude', '2025-11-30 16:00:00', '2025-11-30 16:30:00', NULL, 0, NULL, false, NULL);
            
            INSERT INTO tours.""Messages""(""Id"", ""TourProblemId"", ""AuthorId"", ""Content"", ""Timestamp"", ""AuthorType"")
            VALUES 
            (-201, -111, -21, 'Driver was extremely rude', '2025-11-30 17:00:00', 0),
            (-202, -111, -11, 'We sincerely apologize', '2025-12-01 09:00:00', 1),
            (-203, -111, -21, 'It happened around 3 PM', '2025-12-01 14:00:00', 0),
            (-204, -111, -11, 'We are investigating', '2025-12-02 10:00:00', 1);
        ");
    }

    [Fact]
    public void Retrieves_all_tour_problems()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>();

        // Act
        var result = service.GetAll();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(9);
        result.ShouldContain(p => p.Status == 0); // Open
        result.ShouldContain(p => p.Status == 1); // Resolved
        result.ShouldContain(p => p.Status == 2); // Unresolved
    }

    [Fact]
    public void Retrieves_problem_by_id_with_tour_name()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>();

        // Act
        var result = service.GetById(-101);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-101);
        result.TouristId.ShouldBe(-21);
        result.AuthorId.ShouldBe(-11);
        result.TourName.ShouldNotBeNullOrEmpty(); // Tour name populated
        result.IsOverdue.ShouldBeTrue();
        result.DaysOpen.ShouldBeGreaterThan(5);
    }

    [Fact]
    public void Retrieves_problem_with_messages()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>();

        // Act
        var result = service.GetById(-111);

        // Assert
        result.ShouldNotBeNull();
        result.Messages.ShouldNotBeEmpty();
        result.Messages.Count.ShouldBe(4);
        result.Messages.ShouldContain(m => m.AuthorType == 0); // Tourist
        result.Messages.ShouldContain(m => m.AuthorType == 1); // Author
    }

    [Fact]
    public void Retrieves_overdue_problems_with_default_threshold()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>();

        // Act
        var result = service.GetOverdue(5);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldAllBe(p => p.Status == 0); // Only Open status
        result.ShouldAllBe(p => p.IsOverdue == true);
        result.ShouldAllBe(p => p.DaysOpen >= 5);
        
        // Should contain overdue problems
        result.ShouldContain(p => p.Id == -101);
        result.ShouldContain(p => p.Id == -102);
        result.ShouldContain(p => p.Id == -103);
        result.ShouldContain(p => p.Id == -104);
        result.ShouldContain(p => p.Id == -111);
    }

    [Fact]
    public void Retrieves_overdue_problems_with_custom_threshold()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>();

        // Act
        var result = service.GetOverdue(10);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldAllBe(p => p.DaysOpen >= 10);
        
        // Only problem -104 (20 days old) should be returned
        result.ShouldContain(p => p.Id == -104);
        result.ShouldNotContain(p => p.Id == -101); // 10 days - at threshold
        result.ShouldNotContain(p => p.Id == -102); // 7 days
        result.ShouldNotContain(p => p.Id == -103); // 6 days
    }

    [Fact]
    public void GetOverdue_does_not_return_resolved_problems()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>();

        // Act
        var result = service.GetOverdue(5);

        // Assert
        result.ShouldNotContain(p => p.Status == 1); // No Resolved
        result.ShouldNotContain(p => p.Status == 2); // No Unresolved
        result.ShouldNotContain(p => p.Id == -108); // Resolved problem
        result.ShouldNotContain(p => p.Id == -110); // Unresolved problem
    }

    [Fact]
    public void GetOverdue_does_not_return_fresh_problems()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>();

        // Act
        var result = service.GetOverdue(5);

        // Assert
        result.ShouldNotContain(p => p.Id == -105); // 2 days old
        result.ShouldNotContain(p => p.Id == -106); // 1 day old
    }

    [Fact]
    public void Calculates_days_open_correctly()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>();

        // Act
        var freshProblem = service.GetById(-105);
        var oldProblem = service.GetById(-104);

        // Assert
        freshProblem.DaysOpen.ShouldBeLessThan(5);
        oldProblem.DaysOpen.ShouldBeGreaterThan(15);
    }

    [Fact]
    public void IsOverdue_flag_is_accurate()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>();

        // Act
        var allProblems = service.GetAll();

        // Assert
        var overdueProblem = allProblems.First(p => p.Id == -101);
        overdueProblem.IsOverdue.ShouldBeTrue();

        var freshProblem = allProblems.First(p => p.Id == -105);
        freshProblem.IsOverdue.ShouldBeFalse();

        var resolvedProblem = allProblems.First(p => p.Id == -108);
        resolvedProblem.IsOverdue.ShouldBeFalse(); // Resolved problems not overdue
    }
}
