using Application.Common.Abstractions.Messaging;

namespace Application.UserNotifications.UpdateUserNotifications;

public sealed record UpdateUserNotificationsCommand : ICommand<bool>
{
    public List<KeyValuePair<int, bool>> Notifications { get; init; }
}
