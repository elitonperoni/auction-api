using Application.Common.Abstractions.Messaging;
using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Features.Auctions.Commands.SendBid;
using Domain.Events;
using SharedKernel;

namespace Auction.Worker.Consumer;

public class BidPlacedHandler(
    ICommandHandler<SendBidCommand, SendBidDtoResponse> handler,
    ICacheService cacheService)
{
    public async Task<object> Handle(BidPlaced msg, CancellationToken cancellationToken)
    {
        var command = new SendBidCommand
        {
            AuctionId = msg.AuctionId,
            BidPrice = msg.Amount,
            UserId = msg.UserId,
        };

        Result<SendBidDtoResponse> responseBid = await handler.Handle(command, cancellationToken);

        if (responseBid.IsFailure)
        {
            return new NotificationEvent(
                msg.CallerId.ToString(),
                responseBid.Error.Description);
        }

        await cacheService.AddNotificationAsync(responseBid.Value.AuctionOwnerId, new NotificationItem
        {
            AuctionId = command.AuctionId,
            Message = responseBid.Value.MessageToOwner
        });

        return new BidProcessedEvent(
            responseBid.Value.AuctionId,
            responseBid.Value.Amount,
            responseBid.Value.TotalBids,
            responseBid.Value.LastBidderId,
            responseBid.Value.LastBidderNamer,
            responseBid.Value.AuctionOwnerId,
            responseBid.Value.MessageToOwner,
            responseBid.Value.DescriptionDetail,
            responseBid.Value.Date);
    }
}
