namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface INotificationRepository
{
    Notification Create(Notification notification);
    Notification? GetById(long id);
    List<Notification> GetByRecipientId(long recipientId);
    int GetUnreadCountByRecipientId(long recipientId);
    int MarkAllAsReadByRecipientId(long recipientId);
    List<Notification> GetByRecipientIdPaginated(long recipientId, int page, int pageSize, out int totalCount);
    Notification Update(Notification notification);
}