using AuctionApi.Hubs;
using Domain.Events;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace AuctionApi.Consumer;

public class BidProcessedConsumer : IConsumer<BidProcessedEvent>
{
    private readonly IHubContext<AuctionHub> _hubContext;

    public BidProcessedConsumer(IHubContext<AuctionHub> hubContext, ILogger<BidProcessedConsumer> logger)
    {
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<BidProcessedEvent> context)
    {
        BidProcessedEvent bidResult = context.Message;
       
        await _hubContext.Clients.Group(bidResult.AuctionId.ToString())
            .SendAsync(ChannelNames.ReceiveNewBid,
                bidResult.AuctionId,
                bidResult.TotalAmount,
                bidResult.TotalBids,
                bidResult.LastBidderId,
                bidResult.LastBidderNamer,
                DateTime.UtcNow, 
                context.CancellationToken);
    }
}
