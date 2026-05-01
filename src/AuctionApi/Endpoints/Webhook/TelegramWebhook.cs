using Application.Abstractions.Messaging;
using Application.DTOs;
using Application.Notifications.TelegramNotification;
using Application.Users.Notifications;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

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
