using Application.Common.Abstractions.Messaging;
using Application.Features.Notifications.Commands.TelegramNotification;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuctionApi.Endpoints.Webhook;

internal sealed class TelegramWebhook : IEndpoint
{
    private const string SecretTokenHeader = "X-Telegram-Bot-Api-Secret-Token";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("webhook/telegram", async (
            [FromBody] TelegramUpdateDtoRequest update,
            HttpRequest request,
            IOptions<SecretsApi> options,
            ICommandHandler<TelegramBotMessageCommand, bool> handler,
            CancellationToken cancellationToken) =>
        {
            if (!IsValidSecret(request.Headers[SecretTokenHeader].ToString(), options.Value.TelegramWebhookSecret))
            {
                return Results.Unauthorized();
            }

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
        .WithTags(Tags.Notifications)
        .AllowAnonymous();
    }

    // Telegram echoes the secret passed to setWebhook in this header. Fail closed when none is configured.
    private static bool IsValidSecret(string? provided, string expected)
    {
        if (string.IsNullOrEmpty(expected) || string.IsNullOrEmpty(provided))
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(provided),
            Encoding.UTF8.GetBytes(expected));
    }
}
