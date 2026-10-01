using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Commands.LinkTelegramChatIdAccount;

public sealed record LinkTelegramAccountCommand(Guid UserId, string ChatId) : ICommand<bool>;
