using System.Security.Claims;
using Application.Common.Abstractions.Messaging;
using Application.Features.Auctions.Queries.GetDetail;
using Domain.Events;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SharedKernel;
using Wolverine;

namespace AuctionApi.Hubs;

[Authorize]
public class AuctionHub(
    IMessageBus bus,
    IQueryHandler<GetDetailProductQuery, GetDetailProductResponse> getDetailHandler) : Hub
{
    public async Task SendBid(string groupName, string bidValueString)
    {
        if (!Guid.TryParse(Context.User?.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
        {
            await Clients.Caller.SendAsync(ChannelNames.BidError, "Usuário não autenticado.");
            return;
        }

        if (!Guid.TryParse(groupName, out Guid auctionId))
        {
            await Clients.Caller.SendAsync(ChannelNames.BidError, "Leilão inválido.");
            return;
        }

        if (!decimal.TryParse(bidValueString, out decimal bidAmount))
        {
            await Clients.Caller.SendAsync(ChannelNames.BidError, "Valor do lance inválido.");
            return;
        }

        await bus.SendAsync(
            new BidPlaced(
                Context.ConnectionId,
                auctionId,
                userId,
                bidAmount,
                DateTime.UtcNow));
    }

    public async Task JoinAuctionGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync(ChannelNames.ReceiveMessage, nameof(AuctionHub), $"Você entrou no leilão: {groupName}");
    }

    public async Task JoinUserGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task SyncAuctionState(string auctionId)
    {
        if (!Guid.TryParse(auctionId, out Guid id))
        {
            return;
        }

        Result<GetDetailProductResponse> result =
            await getDetailHandler.Handle(new GetDetailProductQuery(id), Context.ConnectionAborted);

        if (result.IsFailure)
        {
            return;
        }

        await Clients.Caller.SendAsync(ChannelNames.FullAuctionState, result.Value, Context.ConnectionAborted);
    }
}
