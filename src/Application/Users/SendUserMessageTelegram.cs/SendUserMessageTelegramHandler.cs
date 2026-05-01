using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Users.SendUserMessageTelegram.cs;

internal sealed class SendUserMessageTelegramHandler(IApplicationDbContext context, ITelegramService telegramService) : ICommandHandler<SendUserMessageTelegramCommand, bool>
{
    public async  Task<Result<bool>> Handle(SendUserMessageTelegramCommand command, CancellationToken cancellationToken)
    {
        bool userCanReceiveTelegramMessages = await context.UserNotifications
            .AnyAsync(p => p.UserId == command.UserId && p.NotificationTypeId == 1, cancellationToken);

        if (userCanReceiveTelegramMessages)
        {
            string? chatId = await context.Users
                .Where(p => p.Id == command.UserId)
                .Select(p => p.TelegramChatId)
                .FirstOrDefaultAsync(cancellationToken);

            if (!string.IsNullOrEmpty(chatId))
            {
                await telegramService.SendMessage(chatId, command.Message);
            }
        }
        return true;
    }
}
