using Application.Abstractions.Messaging;

namespace Application.Notifications.TelegramNotification;

public sealed record TelegramBotMessageCommand(long ChatId, string Text) : ICommand<bool>;
