using Explorer.API.Hubs;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.SignalR;

namespace Explorer.API.Notifications;

public class SignalRNotificationPublisher : INotificationPublisher
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationPublisher(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task PublishAsync(NotificationDto notification)
    {
        try
        {
            var group = notification.RecipientId.ToString();
            Console.WriteLine($"[SignalRPublisher] Sending notification Id={notification.Id} to group={group}");
            await _hubContext.Clients.Group(group).SendAsync("ReceiveNotification", notification);
            Console.WriteLine($"[SignalRPublisher] Send completed for Id={notification.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SignalRPublisher] Publish error for Id={notification?.Id}: {ex}");
            throw;
        }
    }
}
