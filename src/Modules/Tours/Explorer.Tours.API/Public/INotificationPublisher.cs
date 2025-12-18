using Explorer.Tours.API.Dtos;
namespace Explorer.Tours.API.Public;

public interface INotificationPublisher
{
    Task PublishAsync(NotificationDto notification);
}
