using Application.Abstractions.Messaging;

namespace Application.Users.LinkTelegramAccount;

public sealed record LinkTelegramAccountCommand(Guid UserId, string ChatId) : ICommand<bool>;
