using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Command.SendUserMessageTelegram.cs;

public sealed record SendUserMessageTelegramCommand(Guid UserId, string Message) : ICommand<bool>;
