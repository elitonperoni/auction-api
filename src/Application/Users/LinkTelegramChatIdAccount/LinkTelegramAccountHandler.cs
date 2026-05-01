using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Users.LinkTelegramAccount;

internal sealed class LinkTelegramAccountHandler(IApplicationDbContext context) : ICommandHandler<LinkTelegramAccountCommand, bool>
{
    public async Task<Result<bool>> Handle(LinkTelegramAccountCommand command, CancellationToken cancellationToken)
    {
        User? user = await context.Users.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user == null || string.IsNullOrEmpty(command.ChatId))
        {
            return Result.Failure<bool>(UserErrors.NotFound(command.UserId));
        }            

        user.TelegramChatId = command.ChatId;

        UserNotification? userNotification = new()
        {
            UserId = user.Id,
            NotificationTypeId = 1
        };

        user.Raise(new UserLinkTelegramDomainEvent(command.ChatId));

        context.Users.Update(user);
        await context.UserNotifications.AddAsync(userNotification, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
