using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Notifications;

[Collection("Sequential")]
public class NotificationQueryTests : BaseToursIntegrationTest
{
    public NotificationQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Gets_notifications_for_user()
    {
        using var scope = Factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var expectedCount = dbContext.Notifications.Count(n => n.RecipientId == -11);

        var notifications = repository.GetByRecipientId(-11);

        notifications.ShouldNotBeNull();
        notifications.Count.ShouldBe(expectedCount);
        notifications.All(n => n.RecipientId == -11).ShouldBeTrue();
    }

    [Fact]
    public void Gets_unread_count_for_user()
    {
        using var scope = Factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var expectedCount = dbContext.Notifications.Count(n => n.RecipientId == -11 && !n.IsRead);

        var count = repository.GetUnreadCountByRecipientId(-11);

        count.ShouldBe(expectedCount);
    }

    [Fact]
    public void Returns_zero_unread_for_user_with_no_notifications()
    {
        using var scope = Factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

        var count = repository.GetUnreadCountByRecipientId(-23);

        count.ShouldBe(0);
    }

    [Fact]
    public void Gets_notification_by_id()
    {
        using var scope = Factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

        var notification = repository.GetById(-1);

        notification.ShouldNotBeNull();
        notification.RecipientId.ShouldBe(-11);
        notification.Type.ShouldBe(Explorer.Tours.Core.Domain.NotificationType.NewMessage);
        notification.IsRead.ShouldBeFalse();
    }

    [Fact]
    public void Returns_null_for_non_existent_notification()
    {
        using var scope = Factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

        var notification = repository.GetById(999);

        notification.ShouldBeNull();
    }

    [Fact]
    public void Gets_paginated_notifications()
    {
        using var scope = Factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var expectedTotalCount = dbContext.Notifications.Count(n => n.RecipientId == -11);

        var notifications = repository.GetByRecipientIdPaginated(-11, 1, 2, out int totalCount);

        notifications.ShouldNotBeNull();
        notifications.Count.ShouldBe(2);
        totalCount.ShouldBe(expectedTotalCount);
    }
}