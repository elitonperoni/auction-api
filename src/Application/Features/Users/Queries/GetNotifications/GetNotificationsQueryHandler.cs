using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Abstractions.Authentication;
using Application.Common.Abstractions.Messaging;
using Application.Common.DTOs;
using Application.Common.Interfaces;
using SharedKernel;

namespace Application.Features.Users.Queries.GetNotifications;

internal sealed class GetNotificationsQueryHandler(
    ICacheService cacheService,
    IUserContext userContext)
    : IQueryHandler<GetNotificationsQuery, List<NotificationItem>>
{
    public async Task<Result<List<NotificationItem>>> Handle(GetNotificationsQuery query, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        List<NotificationItem> notificationItems = await cacheService.GetNotificationsAsync(userId);

        return Result.Success(notificationItems);
    }
}
