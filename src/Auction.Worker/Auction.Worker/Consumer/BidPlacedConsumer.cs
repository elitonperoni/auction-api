using Application.Abstractions.Messaging;
using Application.AuctionUseCases.SendBid;
using Application.DTOs;
using Application.Interfaces;
using Domain.Auction;
using Domain.Events;
using Infrastructure.Caching;
using MassTransit;
using SharedKernel;

namespace Auction.Worker.Consumer;

public class BidPlacedConsumer(ICommandHandler<SendBidCommand, SendBidDtoResponse> handler,
    INotificationCacheService notificationCacheService,
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

        await notificationCacheService.AddNotificationAsync(responseBid.Value.AuctionOwnerId, new NotificationItem()
        {
            AuctionId = command.AuctionId,
            Message = responseBid.Value.MessageToOwner
        });

        await publishEndpoint.Publish(new BidProcessedEvent(
            responseBid.Value.AuctionId,
            responseBid.Value.Amount,
            responseBid.Value.TotalBids,
            responseBid.Value.LastBidderId,
            responseBid.Value.LastBidderNamer,
            responseBid.Value.AuctionOwnerId,
            responseBid.Value.MessageToOwner,
            responseBid.Value.Date),
            context.CancellationToken);
    }
}
