using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Command.GenerateLinkTelegramBot;

public sealed record GenerateLinkTelegramBotCommand() : ICommand<string>;

