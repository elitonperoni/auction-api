using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Commands.SendUserMessageTelegram;

public sealed record SendUserMessageTelegramCommand(Guid UserId, string Message) : ICommand<bool>;
