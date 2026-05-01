using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Interfaces;
using SharedKernel;

namespace Application.Users.GenerateLinkTelegramBot;

internal sealed class GenerateLinkTelegramBotHandler(INotificationCacheService notificationCacheService,
    IUserContext userContext) : ICommandHandler<GenerateLinkTelegramBotCommand, string>
{
    public async Task<Result<string>> Handle(GenerateLinkTelegramBotCommand command, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;
        string linkBotTelegram = await notificationCacheService.GenerateLinkTokenTelegram(userId);

        string completeLinkTelegram = $"https://t.me/auctionmax_bot?start={linkBotTelegram}";

        return Result.Success(completeLinkTelegram);
    }
}
