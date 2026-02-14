using Application.Abstractions.Messaging;
using Application.DTOs;
using Application.Users.GetById;
using Application.Users.Notifications;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;

namespace AuctionApi.Endpoints.Users;

internal sealed class GetNotifications : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/notifications", async (
            IQueryHandler<GetNotificationsQuery, List<NotificationItem>> handler,
            CancellationToken cancellationToken) =>
        {            
            Result<List<NotificationItem>> result = await handler.Handle(new GetNotificationsQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })        
        .WithTags(Tags.Users)
        .RequireAuthorization();
    }
}
