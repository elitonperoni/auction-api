using Application.Common.DTOs;

namespace Application.Common.Interfaces;

public interface ICacheService
{
    Task AddNotificationAsync(Guid userId, NotificationItem notification);
    Task<Guid?> ConsumeLinkTokenTelegram(string token);
    Task<string> GenerateLinkTokenTelegram(Guid userId);
    Task<List<NotificationItem>> GetNotificationsAsync(Guid userId);
    Task MarkNotificationAsRead(Guid? notificationId = null);
}
