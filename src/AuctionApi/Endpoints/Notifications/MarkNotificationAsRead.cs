using Application.Abstractions.Messaging;
using Application.Notifications.MarkAsReadById;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace AuctionApi.Endpoints.Notifications;

public class MarkNotificationAsRead : IEndpoint
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
