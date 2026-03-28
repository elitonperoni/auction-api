using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Interfaces;
using SharedKernel;

namespace Application.Notifications.MarkAsReadById;

internal sealed class MarkNotificationAsReadByIdHandler(INotificationCacheService notificationCacheService) : ICommandHandler<MarkNotificationAsReadByIdCommand, bool>
{
    public async Task<Result<bool>> Handle(MarkNotificationAsReadByIdCommand command, CancellationToken cancellationToken)
    {
        await notificationCacheService.MarkNoficationAsRead(command.NotificationId);
        return Result.Success(true);
    }
}
