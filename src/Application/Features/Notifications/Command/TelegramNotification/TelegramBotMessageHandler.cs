using System.Globalization;
using Application.Common.Abstractions.Messaging;
using Application.Common.Interfaces;
using Application.Features.Users.Command.LinkTelegramChatIdAccount;
using SharedKernel;

namespace Application.Features.Notifications.Command.TelegramNotification;

internal sealed class TelegramBotMessageHandler(ICacheService cacheService,
    ICommandHandler<LinkTelegramAccountCommand, bool> userCommandHandler) : ICommandHandler<TelegramBotMessageCommand, bool>
{
    public async Task<Result<bool>> Handle(TelegramBotMessageCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Text))
        {
            return true;
        }            

        if (command.Text.StartsWith("/start ", StringComparison.OrdinalIgnoreCase))
        {
            string[] parts = command.Text.Split(' ');

            if (parts.Length == 2)
            {
                string token = parts[1];
                await ProcessLinkTokenAsync(command.ChatId, token, cancellationToken);
            }
        }

        return Result.Success(true);
    }

    private async Task ProcessLinkTokenAsync(long chatId, string token, CancellationToken cancellationToken)
    {
        Guid? userId = await cacheService.ConsumeLinkTokenTelegram(token);

        if (userId.HasValue)
        {   
            await userCommandHandler.Handle(new LinkTelegramAccountCommand(userId.Value, chatId.ToString("D", CultureInfo.InvariantCulture)), cancellationToken);                       
        }
    }
}
