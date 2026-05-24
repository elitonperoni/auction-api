using AuctionApi.Hubs;
using Domain.Events;
using Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace AuctionApi.Consumer;

public class NotificationHandler(
    IHubContext<AuctionHub> hubContext)
{
    public async Task Handle(NotificationEvent msg, CancellationToken cancellationToken)
    {
        await hubContext.Clients.Client(msg.CallerId.ToString())
            .SendAsync(ChannelNames.ReceiveNotification,
                msg.Message,
                cancellationToken);
    }
}
