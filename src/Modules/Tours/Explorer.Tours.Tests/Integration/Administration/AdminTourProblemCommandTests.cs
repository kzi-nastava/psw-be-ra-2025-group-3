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
        stored.Status.ShouldBe(TourProblemStatus.Resolved);
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

    private static AdminTourProblemController CreateController(IServiceScope scope)
    {
        return new AdminTourProblemController(
            scope.ServiceProvider.GetRequiredService<IAdminTourProblemService>())
        {
            ControllerContext = BuildContext("-1") 
        };
    }
}


