using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Application.Interfaces;
using SharedKernel;

namespace Application.Users.Notifications;

internal sealed class GetNotificationsQueryHandler(
    INotificationCacheService notificationCacheService,
    IUserContext userContext)
    : IQueryHandler<GetNotificationsQuery, List<NotificationItem>>
{
    public async Task<Result<List<NotificationItem>>> Handle(GetNotificationsQuery query, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        List<NotificationItem> notificationItems = await notificationCacheService.GetNotificationsAsync(userId);

        return Result.Success(notificationItems);
    }
}
