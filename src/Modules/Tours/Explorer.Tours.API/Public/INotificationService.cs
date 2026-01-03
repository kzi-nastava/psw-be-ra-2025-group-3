using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public;

public interface INotificationService
{
    // Kreiranje notifikacija (koristi TourProblemService)
    void CreateNewMessageNotification(long recipientId, long problemId, string senderType);
    void CreateProblemResolvedNotification(long recipientId, long problemId);
    void CreateProblemUnresolvedNotification(long recipientId, long problemId);
    void CreateWalletTopUpNotification(long recipientId, int amountAc);

    // API metode za frontend
    List<NotificationDto> GetMyNotifications(long userId);
    UnreadCountDto GetUnreadCount(long userId);
    NotificationDto MarkAsRead(long notificationId, long userId);
    MarkAllReadResultDto MarkAllAsRead(long userId);
}