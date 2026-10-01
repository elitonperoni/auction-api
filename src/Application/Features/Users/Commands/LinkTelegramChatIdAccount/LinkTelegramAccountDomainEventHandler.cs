using Application.Common.Interfaces;
using Domain.Users;
using SharedKernel;

namespace Application.Features.Users.Commands.LinkTelegramChatIdAccount;

internal sealed class LinkTelegramAccountDomainEventHandler(ITelegramService telegramService) : IDomainEventHandler<UserLinkTelegramDomainEvent>
{
    public async Task Handle(UserLinkTelegramDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        const string message = "✅ Your account has been linked successfully! From now on you will receive your alerts here.";

        await telegramService.SendMessageAsync(domainEvent.ChatId, message, cancellationToken);
    }
}
