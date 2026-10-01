using Application.Common.Abstractions.Messaging;
using Application.Features.Notifications.Commands.MarkAsReadById;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace AuctionApi.Endpoints.Notifications;

internal sealed class MarkNotificationAsRead : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("notifications/mark-as-read", async (
            ICommandHandler<MarkNotificationAsReadByIdCommand, bool> handler,
            [FromQuery] Guid? notificationId,
            CancellationToken cancellationToken) =>
        {
            Result<bool> result = await handler.Handle(new MarkNotificationAsReadByIdCommand(notificationId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Notifications)
        .RequireAuthorization();
    }
}
