using Application.Common.Abstractions.Messaging;
using Application.Features.Notifications.Command.TelegramNotification;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApi.Endpoints.Webhook;

internal sealed class TelegramWebhook : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("webhook/telegram", async (
            [FromBody] TelegramUpdateDtoRequest update,
            ICommandHandler<TelegramBotMessageCommand, bool> handler,
            CancellationToken cancellationToken) =>
        {
            if (update.Message is null)
            {
                return Results.Ok();
            }

            var command = new TelegramBotMessageCommand(
                update.Message.Chat?.Id ?? 0,
                update.Message.Text ?? string.Empty
            );

            await handler.Handle(command, cancellationToken);

            return Results.Ok();
        })
        .WithTags(Tags.Notifications);
    }
}
