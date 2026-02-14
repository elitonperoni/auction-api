using System.Security.Claims;
using Domain.Events;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AuctionApi.Hubs;

[Authorize]
public class AuctionHub(IPublishEndpoint publishEndpoint) : Hub
{
    public async Task SendBid(string groupName, string bidValueString)
    {
        Guid? userId = Guid.Parse(Context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

        if (userId == Guid.Empty)
        {
            await Clients.Caller.SendAsync("BidError", "Usuário não autenticado.");
            return;
        }

        if (!decimal.TryParse(bidValueString, out decimal bidAmount))
        {
            await Clients.Caller.SendAsync("BidError", "Valor do lance inválido.");
            return;
        }

        await publishEndpoint.Publish(
            new BidPlaced(
                Context?.ConnectionId?.ToString() ?? "",
                Guid.Parse(groupName), 
                userId.Value, bidAmount, 
                DateTime.UtcNow));

        //await LoadTest(bidAmount, groupName, userId.Value);        
    }

    //private async Task LoadTest(decimal bidAmount, string groupName, Guid userId)
    //{
    //    List<Task> listTasks = [];

    //    decimal mockBidAmount = bidAmount;

    //    DateTime dateTime = DateTime.UtcNow;
    //    for (int i = 0; i < 50; i++)
    //    {
    //        mockBidAmount += 500m;
    //        listTasks.Add(publishEndpoint.Publish(new BidPlaced(Guid.Parse(groupName), userId, mockBidAmount, dateTime)));
    //    }

    //    await Task.WhenAll(listTasks);
    //}

    public async Task JoinAuctionGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync("ReceiveMessage", "AuctionHub", $"Você entrou no leilão: {groupName}");
    }

    public async Task JoinUserGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);        
    }

    public async Task SyncAuctionState(string auctionId)
    {
        // 1. Buscar o estado ATUAL do banco (via serviço)
        //Result<Guid> response = await handler.Handle(new CreateAuctionBidCommand
        //{
        //    AuctionId = Guid.Parse(auctionId),
        //    UserId = Guid.Empty,
        //    BidPrice = 0m,
        //}, CancellationToken.None);

        //if (response != null)
        //{
            // 2. Enviar o estado SÓ PARA O CLIENTE QUE PEDIU
            await Clients.Caller.SendAsync("FullAuctionState", Guid.NewGuid().ToString());
        //}
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
