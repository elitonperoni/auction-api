using Application.Abstractions.Messaging;
using Application.UserNotifications.UpdateUserNotifications;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;

namespace AuctionApi.Endpoints.Users;
internal sealed class UpdateUserNotifications : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/save-notifications", async (
            List<KeyValuePair<int, bool>> request,
            ICommandHandler<UpdateUserNotificationsCommand, bool> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateUserNotificationsCommand() { Notifications = request };

            Result<bool> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
