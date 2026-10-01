using Application.Common.Abstractions.Messaging;

namespace Application.Features.Notifications.Commands.MarkAsReadById;

public sealed record MarkNotificationAsReadByIdCommand(Guid? NotificationId) : ICommand<bool>;
