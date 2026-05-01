using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.UserNotifications.UpdateUserNotifications;

internal sealed class UpdateUserNotificationsHandler(IApplicationDbContext context,
    IUserContext userContext) : ICommandHandler<UpdateUserNotificationsCommand, bool>
{
    public async Task<Result<bool>> Handle(UpdateUserNotificationsCommand command, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        List<UserNotification> userNotifications = await context.UserNotifications
            .Where(p => p.UserId == userId).ToListAsync(cancellationToken);    

        foreach (KeyValuePair<int, bool> notification in command.Notifications)
        {
            UserNotification? userNotification = userNotifications
                .Find(p => p.NotificationTypeId == notification.Key);

            if (userNotification is null)
            {
                context.UserNotifications.Add(new UserNotification
                {
                    UserId = userId,
                    NotificationTypeId = notification.Key
                });
            }
            else if (!notification.Value)
            {
                context.UserNotifications.Remove(userNotification);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
