using Application.Abstractions.Messaging;

namespace Application.Notifications.MarkAsReadById;

public sealed record MarkNotificationAsReadByIdCommand(Guid? NotificationId) : ICommand<bool>;
