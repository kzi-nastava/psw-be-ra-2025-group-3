using Explorer.Tours.Core.Domain;
using Shouldly;

namespace Explorer.Tours.Tests.Unit;

public class TourExecutionLocationTests
{
    [Fact]
    public void Completes_key_point_when_tourist_is_near()
    {
        // Arrange
        var execution = new TourExecution(21, 2, 45.2500, 19.8300);

        // ← KLJUČNO: Dodaj Id ručno za Unit test!
        var keyPoint1 = new KeyPoint(2, "KP1", "First point", "img1.jpg", "Secret1", 45.2510, 19.8310);
        typeof(KeyPoint).GetProperty("Id").SetValue(keyPoint1, 101L); // Postavi Id

        var keyPoint2 = new KeyPoint(2, "KP2", "Second point", "img2.jpg", "Secret2", 45.2600, 19.8400);
        typeof(KeyPoint).GetProperty("Id").SetValue(keyPoint2, 102L);

        var keyPoints = new List<KeyPoint> { keyPoint1, keyPoint2 };

        var initialLastActivity = execution.LastActivity;

        // Act
        var result = execution.CheckLocationProgress(45.2510, 19.8310, keyPoints);

        // Assert
        result.ShouldBeTrue();
        execution.CompletedKeyPoints.Count.ShouldBe(1);
        execution.CompletedKeyPoints[0].KeyPointId.ShouldBe(101L);
        execution.LastActivity.ShouldBeGreaterThan(initialLastActivity);
    }

    [Fact]
    public void Does_not_complete_key_point_when_tourist_is_far()
    {
        // Arrange
        var execution = new TourExecution(21, 2, 45.2500, 19.8300);

        var keyPoint1 = new KeyPoint(2, "KP1", "First point", "img1.jpg", "Secret1", 45.2600, 19.8400);
        typeof(KeyPoint).GetProperty("Id").SetValue(keyPoint1, 101L);

        var keyPoint2 = new KeyPoint(2, "KP2", "Second point", "img2.jpg", "Secret2", 45.2700, 19.8500);
        typeof(KeyPoint).GetProperty("Id").SetValue(keyPoint2, 102L);

        var keyPoints = new List<KeyPoint> { keyPoint1, keyPoint2 };

        var initialLastActivity = execution.LastActivity;

        // Act
        var result = execution.CheckLocationProgress(45.2500, 19.8300, keyPoints);

        // Assert
        result.ShouldBeFalse();
        execution.CompletedKeyPoints.Count.ShouldBe(0);
        execution.LastActivity.ShouldBeGreaterThan(initialLastActivity);
    }

    [Fact]
    public void Does_not_complete_same_key_point_twice()
    {
        // Arrange
        var execution = new TourExecution(21, 2, 45.2500, 19.8300);

        var keyPoint1 = new KeyPoint(2, "KP1", "First point", "img1.jpg", "Secret1", 45.2510, 19.8310);
        typeof(KeyPoint).GetProperty("Id").SetValue(keyPoint1, 101L);

        var keyPoints = new List<KeyPoint> { keyPoint1 };

        // Act - Prvi poziv
        execution.CheckLocationProgress(45.2510, 19.8310, keyPoints);

        // Act - Drugi poziv
        var result = execution.CheckLocationProgress(45.2510, 19.8310, keyPoints);

        // Assert
        result.ShouldBeFalse();
        execution.CompletedKeyPoints.Count.ShouldBe(1);
    }

    [Fact]
    public void Updates_last_activity_on_every_check()
    {
        // Arrange
        var execution = new TourExecution(21, 2, 45.2500, 19.8300);

        var keyPoint1 = new KeyPoint(2, "KP1", "First point", "img1.jpg", "Secret1", 45.2600, 19.8400);
        typeof(KeyPoint).GetProperty("Id").SetValue(keyPoint1, 101L);

        var keyPoints = new List<KeyPoint> { keyPoint1 };

        var activity1 = execution.LastActivity;
        Thread.Sleep(100);

        // Act
        execution.CheckLocationProgress(45.2500, 19.8300, keyPoints);
        var activity2 = execution.LastActivity;
        Thread.Sleep(100);

        execution.CheckLocationProgress(45.2500, 19.8300, keyPoints);
        var activity3 = execution.LastActivity;

        // Assert
        activity2.ShouldBeGreaterThan(activity1);
        activity3.ShouldBeGreaterThan(activity2);
    }

    [Fact]
    public void Completes_multiple_key_points_in_sequence()
    {
        // Arrange
        var execution = new TourExecution(21, 2, 45.2500, 19.8300);

        var keyPoint1 = new KeyPoint(2, "KP1", "First point", "img1.jpg", "Secret1", 45.2510, 19.8310);
        typeof(KeyPoint).GetProperty("Id").SetValue(keyPoint1, 101L);

        var keyPoint2 = new KeyPoint(2, "KP2", "Second point", "img2.jpg", "Secret2", 45.2520, 19.8320);
        typeof(KeyPoint).GetProperty("Id").SetValue(keyPoint2, 102L);

        var keyPoint3 = new KeyPoint(2, "KP3", "Third point", "img3.jpg", "Secret3", 45.2530, 19.8330);
        typeof(KeyPoint).GetProperty("Id").SetValue(keyPoint3, 103L);

        var keyPoints = new List<KeyPoint> { keyPoint1, keyPoint2, keyPoint3 };

        // Act
        var result1 = execution.CheckLocationProgress(45.2510, 19.8310, keyPoints);
        var result2 = execution.CheckLocationProgress(45.2520, 19.8320, keyPoints);
        var result3 = execution.CheckLocationProgress(45.2530, 19.8330, keyPoints);

        // Assert
        result1.ShouldBeTrue();
        result2.ShouldBeTrue();
        result3.ShouldBeTrue();
        execution.CompletedKeyPoints.Count.ShouldBe(3);
    }
}
