using Application.Common.Abstractions.Messaging;
using Application.Common.DTOs;
using Application.Features.Users.Queries.GetNotifications;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;

namespace AuctionApi.Endpoints.Notifications;

internal sealed class GetNotifications : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("notifications", async (
            IQueryHandler<GetNotificationsQuery, List<NotificationItem>> handler,
            CancellationToken cancellationToken) =>
        {            
            Result<List<NotificationItem>> result = await handler.Handle(new GetNotificationsQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })        
        .WithTags(Tags.Notifications)
        .RequireAuthorization();
    }
}
