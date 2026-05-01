using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Interfaces;
using Application.Users.SendUserMessageTelegram.cs;
using AuctionApi.Hubs;
using Domain.Entities;
using Domain.Events;
using Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AuctionApi.Consumer;

public class BidProcessedConsumer(IHubContext<AuctionHub> hubContext,
    ICommandHandler<SendUserMessageTelegramCommand, bool> sendUserMessageTelegramHandler) : IConsumer<BidProcessedEvent>
{
    public async Task Consume(ConsumeContext<BidProcessedEvent> context)
    {
        BidProcessedEvent bidResult = context.Message;
       
        await hubContext.Clients.Group(bidResult.AuctionId.ToString())
            .SendAsync(ChannelNames.ReceiveNewBid,
                bidResult.AuctionId,
                bidResult.TotalAmount,
                bidResult.TotalBids,
                bidResult.LastBidderId,
                bidResult.LastBidderNamer,
                DateTime.UtcNow, 
                context.CancellationToken);

        await hubContext.Clients.Group(bidResult.AuctionOwnerId.ToString())
            .SendAsync(ChannelNames.ReceiveUserNotification,
                bidResult.AuctionId,
                bidResult.MessageToOwner,
                context.CancellationToken);

        await sendUserMessageTelegramHandler.Handle(
            new SendUserMessageTelegramCommand(bidResult.AuctionOwnerId, bidResult.DescriptionDetail), 
            context.CancellationToken);
    }
}
