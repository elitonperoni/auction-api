using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Commands.GenerateLinkTelegramBot;

public sealed record GenerateLinkTelegramBotCommand() : ICommand<string>;

