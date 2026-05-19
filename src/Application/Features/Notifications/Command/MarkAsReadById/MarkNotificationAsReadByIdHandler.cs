using Application.Common.Abstractions.Messaging;
using Application.Common.Interfaces;
using SharedKernel;

namespace Application.Features.Notifications.Command.MarkAsReadById;

internal sealed class MarkNotificationAsReadByIdHandler(INotificationCacheService notificationCacheService) : ICommandHandler<MarkNotificationAsReadByIdCommand, bool>
{
    public async Task<Result<bool>> Handle(MarkNotificationAsReadByIdCommand command, CancellationToken cancellationToken)
    {
        await notificationCacheService.MarkNoficationAsRead(command.NotificationId);
        return Result.Success(true);
    }
}
