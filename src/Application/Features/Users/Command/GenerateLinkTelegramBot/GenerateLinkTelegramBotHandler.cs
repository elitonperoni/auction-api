using Application.Common.Abstractions.Authentication;
using Application.Common.Abstractions.Messaging;
using Application.Common.Interfaces;
using SharedKernel;

namespace Application.Features.Users.Command.GenerateLinkTelegramBot;

internal sealed class GenerateLinkTelegramBotHandler(ICacheService cacheService,
    IUserContext userContext) : ICommandHandler<GenerateLinkTelegramBotCommand, string>
{
    public async Task<Result<string>> Handle(GenerateLinkTelegramBotCommand command, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;
        string linkBotTelegram = await cacheService.GenerateLinkTokenTelegram(userId);

        string completeLinkTelegram = $"https://t.me/auctionmax_bot?start={linkBotTelegram}";

        return Result.Success(completeLinkTelegram);
    }
}
