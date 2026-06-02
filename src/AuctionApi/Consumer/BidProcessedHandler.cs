using Application.Common.Abstractions.Messaging;
using Application.Features.Users.Command.SendUserMessageTelegram.cs;
using AuctionApi.Hubs;
using Domain.Events;
using Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace AuctionApi.Consumer;

public class BidProcessedHandler(
    IHubContext<AuctionHub> hubContext,
    ICommandHandler<SendUserMessageTelegramCommand, bool> sendUserMessageTelegramHandler)
{
    public async Task Handle(BidProcessedEvent msg, CancellationToken cancellationToken)
    {
        await hubContext.Clients.Group(msg.AuctionId.ToString())
            .SendAsync(ChannelNames.ReceiveNewBid,
                msg.AuctionId,
                msg.TotalAmount,
                msg.TotalBids,
                msg.LastBidderId,
                msg.LastBidderNamer,
                DateTime.UtcNow,
                cancellationToken);

        await hubContext.Clients.Group(msg.AuctionOwnerId.ToString())
            .SendAsync(ChannelNames.ReceiveUserNotification,
                msg.AuctionId,
                msg.MessageToOwner,
                cancellationToken);

        await sendUserMessageTelegramHandler.Handle(
            new SendUserMessageTelegramCommand(msg.AuctionOwnerId, msg.DescriptionDetail),
            cancellationToken);
    }
}
