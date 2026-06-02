using Application.Common.Abstractions.Messaging;

namespace Application.Features.Notifications.Command.MarkAsReadById;

public sealed record MarkNotificationAsReadByIdCommand(Guid? NotificationId) : ICommand<bool>;
