using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class NotificationDbRepository : INotificationRepository
{
    private readonly ToursContext _context;

    public NotificationDbRepository(ToursContext context)
    {
        _context = context;
    }

    public Notification Create(Notification notification)
    {
        _context.Notifications.Add(notification);
        _context.SaveChanges();
        return notification;
    }

    public Notification? GetById(long id)
    {
        return _context.Notifications.FirstOrDefault(n => n.Id == id);
    }

    public List<Notification> GetByRecipientId(long recipientId)
    {
        return _context.Notifications
            .Where(n => n.RecipientId == recipientId)
            .OrderByDescending(n => n.CreatedAt)
            .ToList();
    }

    public int GetUnreadCountByRecipientId(long recipientId)
    {
        return _context.Notifications
            .Count(n => n.RecipientId == recipientId && !n.IsRead);
    }

    public int MarkAllAsReadByRecipientId(long recipientId)
    {
        var unreadNotifications = _context.Notifications
            .Where(n => n.RecipientId == recipientId && !n.IsRead)
            .ToList();

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
        }

        _context.SaveChanges();
        return unreadNotifications.Count;
    }

    public List<Notification> GetByRecipientIdPaginated(long recipientId, int page, int pageSize, out int totalCount)
    {
        var query = _context.Notifications
            .Where(n => n.RecipientId == recipientId)
            .OrderByDescending(n => n.CreatedAt);

        totalCount = query.Count();

        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public Notification Update(Notification notification)
    {
        _context.Notifications.Update(notification);
        _context.SaveChanges();
        return notification;
    }
}