using Application.Abstractions.Messaging;

namespace Application.Users.GenerateLinkTelegramBot;

public sealed record GenerateLinkTelegramBotCommand() : ICommand<string>;

