using System;
using System.Linq;
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

        var now = DateTime.UtcNow.Date; // ← Dodaj .Date ovde!
        var date10DaysAgo = now.AddDays(-10).ToString("yyyy-MM-dd HH:mm:ss");
        var date7DaysAgo = now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss");
        var date6DaysAgo = now.AddDays(-6).ToString("yyyy-MM-dd HH:mm:ss");
        var date20DaysAgo = now.AddDays(-20).ToString("yyyy-MM-dd HH:mm:ss");
        var date2DaysAgo = now.AddDays(-2).ToString("yyyy-MM-dd HH:mm:ss");
        var date1DayAgo = now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
        var date8DaysAgo = now.AddDays(-8).ToString("yyyy-MM-dd HH:mm:ss");
        var date19DaysAgo = now.AddDays(-19).ToString("yyyy-MM-dd HH:mm:ss");
        var date9DaysAgo = now.AddDays(-9).ToString("yyyy-MM-dd HH:mm:ss");

        dbContext.Database.ExecuteSqlRaw($@"
            DELETE FROM tours.""Messages"";
            DELETE FROM tours.""TourProblems"";
            
            INSERT INTO tours.""TourProblems""(
                ""Id"", ""TourId"", ""TouristId"", ""AuthorId"", ""Category"", ""Priority"", 
                ""Description"", ""Time"", ""CreatedAt"", ""UpdatedAt"", 
                ""Status"", ""ResolvedByTouristComment"", ""IsHighlighted"", ""AdminDeadline""
            )
            VALUES 
            (-101, -1, -21, -11, 0, 3, 'Bus never arrived at pickup location', '{date10DaysAgo}', '{date10DaysAgo}', NULL, 0, NULL, false, NULL),
            (-102, -1, -21, -11, 1, 2, 'Hotel room was not available', '{date7DaysAgo}', '{date7DaysAgo}', NULL, 0, NULL, false, NULL),
            (-103, -2, -22, -12, 2, 1, 'Tour guide did not show up', '{date6DaysAgo}', '{date6DaysAgo}', NULL, 0, NULL, false, NULL),
            (-104, -1, -22, -11, 3, 3, 'Main attraction was closed', '{date20DaysAgo}', '{date20DaysAgo}', NULL, 0, NULL, false, NULL),
            (-105, -1, -21, -11, 4, 1, 'Lunch portion was smaller', '{date2DaysAgo}', '{date2DaysAgo}', NULL, 0, NULL, false, NULL),
            (-106, -2, -22, -12, 0, 2, 'Bus had no air conditioning', '{date1DayAgo}', '{date1DayAgo}', NULL, 0, NULL, false, NULL),
            (-108, -1, -21, -11, 1, 2, 'Hotel changed our room', '{date8DaysAgo}', '{date8DaysAgo}', '{now.AddDays(-6):yyyy-MM-dd HH:mm:ss}', 1, 'Thank you!', false, NULL),
            (-110, -1, -21, -11, 3, 3, 'Promised refund never received', '{date19DaysAgo}', '{date19DaysAgo}', '{now.AddDays(-11):yyyy-MM-dd HH:mm:ss}', 2, 'Still waiting', false, NULL),
            (-111, -1, -21, -11, 0, 2, 'Bus driver was rude', '{date9DaysAgo}', '{date9DaysAgo}', NULL, 0, NULL, false, NULL);
            
            INSERT INTO tours.""Messages""(""Id"", ""TourProblemId"", ""AuthorId"", ""Content"", ""Timestamp"", ""AuthorType"")
            VALUES 
            (-201, -111, -21, 'Driver was extremely rude', '{now.AddDays(-9).AddHours(1):yyyy-MM-dd HH:mm:ss}', 0),
            (-202, -111, -11, 'We sincerely apologize', '{now.AddDays(-8).AddHours(15):yyyy-MM-dd HH:mm:ss}', 1),
            (-203, -111, -21, 'It happened around 3 PM', '{now.AddDays(-8).AddHours(10):yyyy-MM-dd HH:mm:ss}', 0),
            (-204, -111, -11, 'We are investigating', '{now.AddDays(-7).AddHours(14):yyyy-MM-dd HH:mm:ss}', 1);
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
        result.TourName.ShouldNotBeNullOrEmpty();
        result.IsOverdue.ShouldBeTrue();
        result.DaysOpen.ShouldBeGreaterThanOrEqualTo(10); // Može biti 10 ili više
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
        result.Messages.ShouldContain(m => m.AuthorType == 0);
        result.Messages.ShouldContain(m => m.AuthorType == 1);
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
        result.ShouldAllBe(p => p.Status == 0);
        result.ShouldAllBe(p => p.IsOverdue == true);
        result.ShouldAllBe(p => p.DaysOpen > 5); // Striktno veće od 5
        
        // Should contain overdue problems (6+ days)
        result.ShouldContain(p => p.Id == -102); // 7 dana
        result.ShouldContain(p => p.Id == -103); // 6 dana
        result.ShouldContain(p => p.Id == -104); // 20 dana
        result.ShouldContain(p => p.Id == -111); // 9 dana
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
        result.ShouldAllBe(p => p.DaysOpen > 10); // Striktno veće od 10
        
        // Only problem -104 (20 days old) should be returned
        result.ShouldContain(p => p.Id == -104);
        result.ShouldNotContain(p => p.Id == -101); // 10 days - at threshold
        result.ShouldNotContain(p => p.Id == -102); // 7 days
        result.ShouldNotContain(p => p.Id == -103); // 6 days
        result.ShouldNotContain(p => p.Id == -111); // 9 days
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
        result.ShouldNotContain(p => p.Status == 1);
        result.ShouldNotContain(p => p.Status == 2);
        result.ShouldNotContain(p => p.Id == -108);
        result.ShouldNotContain(p => p.Id == -110);
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
        result.ShouldNotContain(p => p.Id == -105); // 2 days
        result.ShouldNotContain(p => p.Id == -106); // 1 day
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
        resolvedProblem.IsOverdue.ShouldBeFalse();
    }
}
