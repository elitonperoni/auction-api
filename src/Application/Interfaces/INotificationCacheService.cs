using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces;

public interface INotificationCacheService
{
    Task AddNotificationAsync(Guid userId, NotificationItem notification);
    Task<Guid?> ConsumeLinkTokenTelegram(string token);
    Task<string> GenerateLinkTokenTelegram(Guid userId);
    Task<List<NotificationItem>> GetNotificationsAsync(Guid userId);
    Task MarkNoficationAsRead(Guid? notificationId = null);
}
