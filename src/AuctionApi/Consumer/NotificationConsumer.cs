using AuctionApi.Hubs;
using Domain.Events;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace AuctionApi.Consumer;

public class NotificationConsumer : IConsumer<NotificationEvent>
{
    private readonly IHubContext<AuctionHub> _hubContext;

    public NotificationConsumer(IHubContext<AuctionHub> hubContext, ILogger<NotificationConsumer> logger)
    {
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<NotificationEvent> context)
    {
        NotificationEvent bidResult = context.Message;
        
        await _hubContext.Clients.Client(bidResult.CallerId.ToString())
        .SendAsync(ChannelNames.ReceiveNotification,
            bidResult.Message,
            context.CancellationToken);
    }
}
