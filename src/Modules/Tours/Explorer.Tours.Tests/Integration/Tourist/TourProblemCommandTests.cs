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
            (-1, -1, -21, -11, 0, 2, 'Bus was 30 minutes late at pickup point', '2024-11-15 08:30:00', '2024-11-15 09:00:00', NULL, 0, NULL, false, NULL),
            (-2, -1, -21, -11, 2, 1, 'Tour guide spoke too quietly, hard to hear explanations', '2024-11-15 10:15:00', '2024-11-15 12:00:00', NULL, 0, NULL, false, NULL),
            (-3, -2, -22, -12, 3, 3, 'Main attraction was closed without prior notice', '2024-11-14 14:00:00', '2024-11-14 15:30:00', NULL, 0, NULL, false, NULL),
            (-4, -1, -22, -11, 4, 0, 'Lunch portion was smaller than expected', '2024-11-15 12:30:00', '2024-11-15 13:00:00', NULL, 0, NULL, false, NULL),
            (-5, -1, -21, -11, 0, 2, 'Problem for resolving test', '2024-11-15 08:30:00', '2024-11-15 09:00:00', NULL, 0, NULL, false, NULL),
            (-6, -1, -21, -11, 0, 2, 'Problem for unresolving test', '2024-11-15 08:30:00', '2024-11-15 09:00:00', NULL, 0, NULL, false, NULL),
            (-7, -1, -21, -11, 0, 2, 'Problem for message test', '2024-11-15 08:30:00', '2024-11-15 09:00:00', NULL, 0, NULL, false, NULL);
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

        // Act - Tourist -21 creates problem
        var result = service.Create(newProblem, -21);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.TourId.ShouldBe(newProblem.TourId);
        result.TouristId.ShouldBe(-21);
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
        Should.Throw<ArgumentException>(() => service.Create(newProblem, -21));
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

        // Act - Tourist -21 updates their problem
        var result = service.Update(updateDto, -21);

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

        // Act & Assert - Tourist -22 cannot update problem created by -21
        Should.Throw<Exception>(() => service.Update(updateDto, -22));
    }

    [Fact]
    public void Deletes_problem()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act - Tourist -21 deletes their problem
        service.Delete(-2, -21);

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

        // Act & Assert - Tourist -22 cannot delete problem created by -21
        Should.Throw<Exception>(() => service.Delete(-1, -22));
    }

    [Theory]
    [InlineData(-5, -21, "Problem is now resolved, thank you!", 1)] // Tourist -21 marks as resolved
    public void Marks_problem_as_resolved(long problemId, long touristId, string comment, int expectedStatus)
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = service.MarkAsResolved(problemId, comment, touristId);

        // Assert - Response
        result.ShouldNotBeNull();
        result.Status.ShouldBe(expectedStatus);
        result.ResolvedByTouristComment.ShouldBe(comment);

        // Assert - Database
        var storedEntity = dbContext.TourProblems.FirstOrDefault(p => p.Id == problemId);
        storedEntity.ShouldNotBeNull();
        storedEntity.Status.ShouldBe((Explorer.Tours.Core.Domain.TourProblemStatus)expectedStatus);
        storedEntity.ResolvedByTouristComment.ShouldBe(comment);
    }

    [Fact]
    public void Mark_as_resolved_fails_invalid_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        // Act & Assert - Tourist -22 cannot mark problem created by -21
        Should.Throw<Exception>(() => service.MarkAsResolved(-1, "Comment", -22));
    }

    [Fact]
    public void Mark_as_resolved_fails_empty_comment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        // Act & Assert
        Should.Throw<ArgumentException>(() => service.MarkAsResolved(-5, "", -21));
    }

    [Theory]
    [InlineData(-6, -21, "Problem is still not resolved, needs more attention!", 2)] // Tourist -21 marks as unresolved
    public void Marks_problem_as_unresolved(long problemId, long touristId, string comment, int expectedStatus)
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = service.MarkAsUnresolved(problemId, comment, touristId);

        // Assert - Response
        result.ShouldNotBeNull();
        result.Status.ShouldBe(expectedStatus);
        result.ResolvedByTouristComment.ShouldBe(comment);

        // Assert - Database
        var storedEntity = dbContext.TourProblems.FirstOrDefault(p => p.Id == problemId);
        storedEntity.ShouldNotBeNull();
        storedEntity.Status.ShouldBe((Explorer.Tours.Core.Domain.TourProblemStatus)expectedStatus);
        storedEntity.ResolvedByTouristComment.ShouldBe(comment);
    }

    [Fact]
    public void Mark_as_unresolved_fails_invalid_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        // Act & Assert - Tourist -22 cannot mark problem created by -21
        Should.Throw<Exception>(() => service.MarkAsUnresolved(-1, "Comment", -22));
    }

    [Fact]
    public void Mark_as_unresolved_fails_empty_comment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        // Act & Assert
        Should.Throw<ArgumentException>(() => service.MarkAsUnresolved(-6, "", -21));
    }

    [Theory]
    [InlineData(-7, -21, "This is a message from tourist", 0)] // Tourist -21 sends message
    [InlineData(-7, -11, "This is a response from tour author", 1)] // Author -11 sends message
    public void Adds_message_to_problem(long problemId, long authorId, string content, int authorType)
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = service.AddMessage(problemId, authorId, content, authorType);

        // Assert - Response
        result.ShouldNotBeNull();
        result.Messages.ShouldNotBeEmpty();
        result.Messages.ShouldContain(m => m.Content == content && m.AuthorId == authorId);

        // Assert - Database
        var storedEntity = dbContext.TourProblems
            .Include(p => p.Messages)
            .FirstOrDefault(p => p.Id == problemId);
        storedEntity.ShouldNotBeNull();
        storedEntity.Messages.ShouldContain(m => m.Content == content && m.AuthorId == authorId);
    }

    [Fact]
    public void Add_message_fails_empty_content()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        // Act & Assert
        Should.Throw<ArgumentException>(() => service.AddMessage(-7, -21, "", 0));
    }

    [Fact]
    public void Add_message_fails_invalid_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        // Act & Assert - Wrong tourist trying to send message as Tourist
        Should.Throw<InvalidOperationException>(() => service.AddMessage(-7, -22, "Invalid message", 0));
    }

    [Fact]
    public void Add_message_fails_invalid_author()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourProblemService>();

        // Act & Assert - Wrong author trying to send message as Author
        Should.Throw<InvalidOperationException>(() => service.AddMessage(-7, -12, "Invalid message", 1));
    }
}