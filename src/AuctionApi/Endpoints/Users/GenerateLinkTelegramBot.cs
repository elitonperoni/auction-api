using Application.Abstractions.Messaging;
using Application.Users.GenerateLinkTelegramBot;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;

namespace AuctionApi.Endpoints.Users;

internal sealed class GenerateLinkTelegramBot : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/generate-link-telegram-bot", async (
            ICommandHandler<GenerateLinkTelegramBotCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            Result<string> result = await handler.Handle(new GenerateLinkTelegramBotCommand(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .RequireAuthorization();
    }
}
