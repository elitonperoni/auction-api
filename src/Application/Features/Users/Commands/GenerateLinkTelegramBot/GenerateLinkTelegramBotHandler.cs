using Application.Common.Abstractions.Authentication;
using Application.Common.Abstractions.Messaging;
using Application.Common.Interfaces;
using Application.Common.Options;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Application.Features.Users.Commands.GenerateLinkTelegramBot;

internal sealed class GenerateLinkTelegramBotHandler(
    ICacheService cacheService,
    IUserContext userContext,
    IOptions<SecretsApi> options) : ICommandHandler<GenerateLinkTelegramBotCommand, string>
{
    public async Task<Result<string>> Handle(GenerateLinkTelegramBotCommand command, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;
        string linkBotTelegram = await cacheService.GenerateLinkTokenTelegram(userId);

        string completeLinkTelegram = $"https://t.me/{options.Value.TelegramBotUsername}?start={linkBotTelegram}";

        return Result.Success(completeLinkTelegram);
    }
}
