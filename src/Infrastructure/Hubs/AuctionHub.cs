using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Hubs;

public class AuctionHub : Hub
{
    public async Task JoinAuctionGroup(string auctionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"leilao-{auctionId}");
    }

    public async Task LeaveAuctionGroup(string auctionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"leilao-{auctionId}");
    }
}
