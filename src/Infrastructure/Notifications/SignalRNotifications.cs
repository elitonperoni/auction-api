using Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

//namespace Infrastructure.Notifications;

//public class SignalRNotifications : IAuctionNotifier
//{
//    private readonly IHubContext<AuctionHub> _hubContext;

//    public SignalRNotifications(IHubContext<AuctionHub> hubContext)
//    {
//        _hubContext = hubContext;
//    }

//    public async Task NotifyNewBid(string auctionId, object bidDto)
//    {
//        await _hubContext.Clients.Group($"leilao-{auctionId}")
//                         .SendAsync("NovoLance", bidDto);
//    }
//}
