using SharedKernel;

namespace Domain.Users;

public sealed record UserLinkTelegramDomainEvent(string ChatId) : IDomainEvent;
