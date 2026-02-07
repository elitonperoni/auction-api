using Application.Abstractions.Messaging;
using Application.AuctionUseCases.SendBid;
using Domain.Events;
using MassTransit;
using SharedKernel;

namespace Auction.Worker.Consumer;

public class BidPlacedConsumer(ICommandHandler<SendBidCommand, SendBidDtoResponse> handler,
    IPublishEndpoint publishEndpoint) : 
    IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {      
        var command = new SendBidCommand()
        {
            AuctionId = context.Message.AuctionId,
            BidPrice = context.Message.Amount,
            UserId = context.Message.UserId,
        };

        Result<SendBidDtoResponse> responseBid = await handler.Handle(command, context.CancellationToken);

        if (responseBid.IsFailure)
        {
            await publishEndpoint.Publish(new NotificationEvent(
               context.Message.CallerId.ToString(),
               responseBid.Error.Description),
               context.CancellationToken);
            return;
        }

        await publishEndpoint.Publish(new BidProcessedEvent(
            responseBid.Value.AuctionId,
            responseBid.Value.Amount,
            responseBid.Value.TotalBids,
            responseBid.Value.LastBidderId,
            responseBid.Value.LastBidderNamer,
            responseBid.Value.Date),
            context.CancellationToken);
    }
}
