using Explorer.API.Controllers;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Notifications;

[Collection("Sequential")]
public class NotificationCommandTests : BaseToursIntegrationTest
{
    public NotificationCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Marks_notification_as_read_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var notification = dbContext.Notifications.FirstOrDefault(n => n.Id == -1);
        notification.ShouldNotBeNull();

        notification.IsRead = false;
        notification.ReadAt = null;
        dbContext.SaveChanges();

        var actionResult = controller.MarkAsRead(-1);

        actionResult.ShouldNotBeNull();
        actionResult.Result.ShouldBeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        var dto = okResult.Value as NotificationDto;
        dto.ShouldNotBeNull();
        dto.IsRead.ShouldBeTrue();
        dto.ReadAt.ShouldNotBeNull();

        dbContext.Entry(notification).Reload();

        notification.IsRead.ShouldBeTrue();
        notification.ReadAt.ShouldNotBeNull();
    }

    [Fact]
    public void Fails_to_mark_other_users_notification_as_read()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        var actionResult = controller.MarkAsRead(-1);

        actionResult.ShouldNotBeNull();
        actionResult.Result.ShouldBeOfType<ObjectResult>();

        var result = actionResult.Result as ObjectResult;
        result.StatusCode.ShouldBe(403);
    }

    [Fact]
    public void Marks_all_notifications_as_read_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var unreadCountBefore = dbContext.Notifications
            .Count(n => n.RecipientId == -11 && !n.IsRead);

        var actionResult = controller.MarkAllAsRead();

        actionResult.ShouldNotBeNull();
        actionResult.Result.ShouldBeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        var dto = okResult.Value as MarkAllReadResultDto;
        dto.ShouldNotBeNull();
        dto.UpdatedCount.ShouldBe(unreadCountBefore);

        var unreadCountAfter = dbContext.Notifications
            .Count(n => n.RecipientId == -11 && !n.IsRead);
        unreadCountAfter.ShouldBe(0);
    }

    [Fact]
    public void Gets_correct_unread_count()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var expectedCount = dbContext.Notifications
            .Count(n => n.RecipientId == -11 && !n.IsRead);

        var actionResult = controller.GetUnreadCount();

        actionResult.ShouldNotBeNull();
        actionResult.Result.ShouldBeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        var dto = okResult.Value as UnreadCountDto;
        dto.ShouldNotBeNull();
        dto.Count.ShouldBe(expectedCount);
    }

    [Fact]
    public void Gets_my_notifications_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var expectedCount = dbContext.Notifications
            .Count(n => n.RecipientId == -11);

        var actionResult = controller.GetMyNotifications();

        actionResult.ShouldNotBeNull();
        actionResult.Result.ShouldBeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        var notifications = okResult.Value as List<NotificationDto>;
        notifications.ShouldNotBeNull();
        notifications.Count.ShouldBe(expectedCount);
        notifications.All(n => n.RecipientId == -11).ShouldBeTrue();
    }

    [Fact]
    public void Returns_empty_list_for_user_with_no_notifications()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-23");

        var actionResult = controller.GetMyNotifications();

        actionResult.ShouldNotBeNull();
        actionResult.Result.ShouldBeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        var notifications = okResult.Value as List<NotificationDto>;
        notifications.ShouldNotBeNull();
        notifications.Count.ShouldBe(0);
    }

    [Fact]
    public void Fails_to_mark_non_existent_notification_as_read()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var actionResult = controller.MarkAsRead(999);

        actionResult.ShouldNotBeNull();
        actionResult.Result.ShouldBeOfType<NotFoundObjectResult>();
    }

    private static NotificationController CreateController(IServiceScope scope, string userId)
    {
        return new NotificationController(
            scope.ServiceProvider.GetRequiredService<INotificationService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }

    private static ControllerContext BuildContext(string userId)
    {
        var user = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim("personId", userId),
                new System.Security.Claims.Claim("id", userId)
            }, "mock"));
        return new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
    }
}