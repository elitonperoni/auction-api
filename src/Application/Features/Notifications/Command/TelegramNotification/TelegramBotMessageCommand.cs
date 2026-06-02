using Application.Common.Abstractions.Messaging;

namespace Application.Features.Notifications.Command.TelegramNotification;

public sealed record TelegramBotMessageCommand(long ChatId, string Text) : ICommand<bool>;
