using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Users;
using SharedKernel;

namespace Application.Users.LinkTelegramAccount;

internal sealed class LinkTelegramAccountDomainEventHandler(ITelegramService telegramService) : IDomainEventHandler<UserLinkTelegramDomainEvent>
{
    public async Task Handle(UserLinkTelegramDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        string message = "✅ Sua conta foi vinculada com sucesso! A partir de agora você receberá seus alertas aqui." ;        
        await telegramService.SendMessage(domainEvent.ChatId, message);
    }
}
