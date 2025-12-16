using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class AdminTourProblemCommandTests : BaseToursIntegrationTest
{
    public AdminTourProblemCommandTests(ToursTestFactory factory) : base(factory) { }


    [Fact]
    public void SetDeadline_success()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var controller = CreateController(scope);

        // Arrange
        var tour = new Tour("Test tour", "Desc", TourDifficulty.Easy, -11);
        dbContext.Tours.Add(tour);
        dbContext.SaveChanges();

        var problem = new TourProblem(tour.Id, -21, -11, ProblemCategory.Other,
            ProblemPriority.Medium, "Test problem for deadline", DateTime.UtcNow.AddDays(-6));

        dbContext.TourProblems.Add(problem);
        dbContext.SaveChanges();

        var dto = new AdminDeadlineDto
        {
            Deadline = DateTime.UtcNow.AddDays(5)
        };

        // Act
        var result = (OkResult)controller.SetDeadline(problem.Id, dto);

        // Assert
        result.StatusCode.ShouldBe(200);

        var stored = dbContext.TourProblems.First(p => p.Id == problem.Id);
        stored.AdminDeadline.ShouldNotBeNull();

        var notification = dbContext.Notifications
            .FirstOrDefault(n => n.RelatedEntityId == problem.Id && n.Type == NotificationType.DeadlineSet);

        notification.ShouldNotBeNull();
    }


    [Fact]
    public void SetDeadline_fails_invalid_problem_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new AdminDeadlineDto
        {
            Deadline = DateTime.UtcNow.AddDays(3)
        };

        Should.Throw<NotFoundException>(() =>
            controller.SetDeadline(-9999, dto)
        );
    }


    [Fact]
    public void CloseProblem_success()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var controller = CreateController(scope);

        var tour = new Tour("Close tour", "Desc", TourDifficulty.Easy, -11);
        dbContext.Tours.Add(tour);
        dbContext.SaveChanges();

        var problem = new TourProblem(tour.Id, -21, -11, ProblemCategory.Other,
            ProblemPriority.Medium, "Close test problem", DateTime.UtcNow.AddDays(-3));

        dbContext.TourProblems.Add(problem);
        dbContext.SaveChanges();

        // Act
        var result = (OkResult)controller.CloseProblem(problem.Id);

        // Assert
        result.StatusCode.ShouldBe(200);

        var stored = dbContext.TourProblems.First(p => p.Id == problem.Id);
        stored.Status.ShouldBe(TourProblemStatus.Closed);
    }


    [Fact]
    public void CloseProblem_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<NotFoundException>(() =>
            controller.CloseProblem(-8888)
        );
    }


    [Fact]
    public void PenalizeAuthor_success()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var controller = CreateController(scope);

        // Arrange
        var tour = new Tour(
            "Penalize tour",
            "Valid description",
            TourDifficulty.Easy,
            -11,
            new List<string> { "test" } 
        );

        dbContext.Tours.Add(tour);
        dbContext.SaveChanges();

        var kp1 = new KeyPoint(
            tour.Id,
            "KP1",
            "Opis KP1",
            "test.png",      
            "secret1",       
            45.0,
            19.0
        );

        var kp2 = new KeyPoint(
            tour.Id,
            "KP2",
            "Opis KP2",
            "test2.png",     
            "secret2",       
            45.1,
            19.1
        );


        tour.KeyPoints.Add(kp1);
        tour.KeyPoints.Add(kp2);

        var duration = new TourDuration(60, TransportType.Walking);
        tour.TourDurations.Add(duration);

        dbContext.SaveChanges();

        tour.Publish();
        dbContext.SaveChanges();

        var problem = new TourProblem(
            tour.Id,
            -21,
            -11,
            ProblemCategory.Other,
            ProblemPriority.High,
            "Penalize test problem",
            DateTime.UtcNow.AddDays(-7)
        );

        dbContext.TourProblems.Add(problem);
        dbContext.SaveChanges();

        // Act
        var result = (OkResult)controller.Penalize(problem.Id);

        // Assert
        result.StatusCode.ShouldBe(200);

        var storedTour = dbContext.Tours.First(t => t.Id == tour.Id);
        storedTour.Status.ShouldBe(TourStatus.Archived);
    }


    [Fact]
    public void PenalizeAuthor_fails_invalid_problem_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<NotFoundException>(() =>
            controller.Penalize(-7777)
        );
    }

    //testovi za dodavanje poruka od strane admina

    [Fact]
    public void Admin_adds_message_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var service = scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>();

        var tour = new Tour("Test tour for message", "Description", TourDifficulty.Easy, -11);
        dbContext.Tours.Add(tour);
        dbContext.SaveChanges();

        var problem = new TourProblem(
            tour.Id,
            -21, // Tourist
            -11, // Author
            ProblemCategory.Transportation,
            ProblemPriority.High,
            "Test problem for admin message",
            DateTime.UtcNow.AddDays(-3)
        );
        dbContext.TourProblems.Add(problem);
        dbContext.SaveChanges();

        // Act
        var result = service.AddAdminMessage(problem.Id, -1, "Administrator is investigating this issue.");

        // Assert
        result.ShouldNotBeNull();
        result.Messages.ShouldNotBeEmpty();
        result.Messages.Count.ShouldBe(1);
        result.Messages.ShouldContain(m => m.AuthorType == 2); // AuthorType.Admin = 2
        result.Messages.ShouldContain(m => m.Content == "Administrator is investigating this issue.");

        // Proveri da li su kreirane notifikacije
        var touristNotification = dbContext.Notifications
            .FirstOrDefault(n => n.RecipientId == -21 && n.RelatedEntityId == problem.Id);
        touristNotification.ShouldNotBeNull();
        touristNotification.Type.ShouldBe(NotificationType.NewMessage);

        var authorNotification = dbContext.Notifications
            .FirstOrDefault(n => n.RecipientId == -11 && n.RelatedEntityId == problem.Id);
        authorNotification.ShouldNotBeNull();
        authorNotification.Type.ShouldBe(NotificationType.NewMessage);
    }

    [Fact]
    public void Admin_adds_message_fails_empty_content()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var service = scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>();

        var tour = new Tour("Test tour", "Desc", TourDifficulty.Easy, -11);
        dbContext.Tours.Add(tour);
        dbContext.SaveChanges();

        var problem = new TourProblem(
            tour.Id,
            -21,
            -11,
            ProblemCategory.Other,
            ProblemPriority.Low,
            "Test problem for empty message",
            DateTime.UtcNow.AddDays(-2)
        );
        dbContext.TourProblems.Add(problem);
        dbContext.SaveChanges();

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            service.AddAdminMessage(problem.Id, -1, "")
        );
    }

    [Fact]
    public void Admin_adds_message_fails_too_long_content()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var service = scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>();

        var tour = new Tour("Test tour", "Desc", TourDifficulty.Easy, -11);
        dbContext.Tours.Add(tour);
        dbContext.SaveChanges();

        var problem = new TourProblem(
            tour.Id,
            -21,
            -11,
            ProblemCategory.Other,
            ProblemPriority.Low,
            "Test problem for long message",
            DateTime.UtcNow.AddDays(-2)
        );
        dbContext.TourProblems.Add(problem);
        dbContext.SaveChanges();

        var longContent = new string('A', 1001); // 1001 karaktera (limit je 1000)

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            service.AddAdminMessage(problem.Id, -1, longContent)
        );
    }

    [Fact]
    public void Admin_adds_message_fails_invalid_problem_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>();

        // Act & Assert
        Should.Throw<NotFoundException>(() =>
            service.AddAdminMessage(-9999, -1, "Test message")
        );
    }

    [Fact]
    public void Admin_adds_multiple_messages_to_same_problem()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var service = scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>();

        var tour = new Tour("Test tour", "Desc", TourDifficulty.Easy, -11);
        dbContext.Tours.Add(tour);
        dbContext.SaveChanges();

        var problem = new TourProblem(
            tour.Id,
            -21,
            -11,
            ProblemCategory.Transportation,
            ProblemPriority.Medium,
            "Test problem for multiple messages",
            DateTime.UtcNow.AddDays(-5)
        );
        dbContext.TourProblems.Add(problem);
        dbContext.SaveChanges();

        // Act - Dodaj prvu poruku
        var result1 = service.AddAdminMessage(problem.Id, -1, "First admin message");

        // Act - Dodaj drugu poruku
        var result2 = service.AddAdminMessage(problem.Id, -1, "Second admin message");

        // Assert
        result2.ShouldNotBeNull();
        result2.Messages.Count.ShouldBe(2);
        result2.Messages.ShouldContain(m => m.Content == "First admin message");
        result2.Messages.ShouldContain(m => m.Content == "Second admin message");
    }

    [Fact]
    public void Admin_message_creates_notifications_for_both_tourist_and_author()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var service = scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>();

        var tour = new Tour("Notification test tour", "Desc", TourDifficulty.Easy, -11);
        dbContext.Tours.Add(tour);
        dbContext.SaveChanges();

        var problem = new TourProblem(
            tour.Id,
            -21, // Tourist
            -11, // Author
            ProblemCategory.Location,
            ProblemPriority.Critical,
            "Problem for notification test",
            DateTime.UtcNow.AddDays(-4)
        );
        dbContext.TourProblems.Add(problem);
        dbContext.SaveChanges();

        // Act
        service.AddAdminMessage(problem.Id, -1, "Admin response for notification test");

        // Assert - Proveri notifikacije
        var notifications = dbContext.Notifications
            .Where(n => n.RelatedEntityId == problem.Id && n.Type == NotificationType.NewMessage)
            .ToList();

        notifications.Count.ShouldBe(2); // Jedna za turista, jedna za autora
        notifications.ShouldContain(n => n.RecipientId == -21); // Turista
        notifications.ShouldContain(n => n.RecipientId == -11); // Autor
    }

    // Helper metoda za kreiranje kontrolera
    private static AdminTourProblemController CreateController(IServiceScope scope)
    {
        return new AdminTourProblemController(
            scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>())
        {
            ControllerContext = BuildContext("-1") 
        };
    }
}


