using System;
using Explorer.Tours.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit;

public class KeyPointCompletionTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999999)]
    public void Creates_valid_key_point_completion(long keyPointId)
    {
        // Arrange
        var completedAt = DateTime.UtcNow;

        // Act
        var completion = new KeyPointCompletion(keyPointId, completedAt);

        // Assert
        completion.ShouldNotBeNull();
        completion.KeyPointId.ShouldBe(keyPointId);
        completion.CompletedAt.ShouldBe(completedAt);
    }

    [Fact]
    public void Fails_to_create_with_zero_keypoint_id()
    {
        // Arrange
        var completedAt = DateTime.UtcNow;

        // Act & Assert
        Should.Throw<ArgumentException>(() => new KeyPointCompletion(0, completedAt))
            .Message.ShouldContain("KeyPoint ID must be valid");
    }

    [Fact]
    public void Two_completions_with_same_data_are_equal()
    {
        // Arrange
        var keyPointId = 5L;
        var completedAt = new DateTime(2025, 1, 15, 10, 30, 0);

        // Act
        var completion1 = new KeyPointCompletion(keyPointId, completedAt);
        var completion2 = new KeyPointCompletion(keyPointId, completedAt);

        // Assert
        completion1.ShouldBe(completion2);
        completion1.Equals(completion2).ShouldBeTrue();
    }

    [Fact]
    public void Two_completions_with_different_keypoint_id_are_not_equal()
    {
        // Arrange
        var completedAt = DateTime.UtcNow;

        // Act
        var completion1 = new KeyPointCompletion(1, completedAt);
        var completion2 = new KeyPointCompletion(2, completedAt);

        // Assert
        completion1.ShouldNotBe(completion2);
    }

    [Fact]
    public void Two_completions_with_different_time_are_not_equal()
    {
        // Arrange
        var keyPointId = 5L;
        var time1 = new DateTime(2025, 1, 15, 10, 30, 0);
        var time2 = new DateTime(2025, 1, 15, 10, 30, 1); // 1 sekunda razlike

        // Act
        var completion1 = new KeyPointCompletion(keyPointId, time1);
        var completion2 = new KeyPointCompletion(keyPointId, time2);

        // Assert
        completion1.ShouldNotBe(completion2);
    }

    [Fact]
    public void Completion_preserves_exact_timestamp()
    {
        // Arrange
        var keyPointId = 1L;
        var exactTime = new DateTime(2025, 12, 15, 14, 25, 33, 123);

        // Act
        var completion = new KeyPointCompletion(keyPointId, exactTime);

        // Assert
        completion.CompletedAt.ShouldBe(exactTime);
        completion.CompletedAt.Millisecond.ShouldBe(123);
    }
}