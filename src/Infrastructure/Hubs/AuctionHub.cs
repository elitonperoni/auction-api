using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.AuctionUseCases.Create;
using Application.AuctionUseCases.SendBid;
using Application.Extensions;
using Application.Todos.Complete;
using Domain.Users;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Infrastructure.Hubs;

[Authorize]
public class AuctionHub(ICommandHandler<CreateAuctionBidCommand, Guid> handler) : Hub
{
    public async Task JoinAuctionGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync("ReceiveMessage", "AuctionHub", $"Você entrou no leilão: {groupName}");
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

    public async Task SendBid(string groupName, string bidValueString)
    {
        var userId = Guid.Parse(Context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        string userName =  Context?.User?.Claims
            .FirstOrDefault(c => c.Type == "name")?
            .Value ?? "Usuário anônimo";

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

        await handler.Handle(new CreateAuctionBidCommand
        {
            AuctionId = Guid.Parse(groupName), 
            UserId = userId,
            BidPrice = bidAmount,
        }, CancellationToken.None);

        decimal newCurrentBid = bidAmount;
        int newTotalBids = 20 + 1;

        string newBidTime = DateTimeExtension.GetCurrentTime();

        //Notifiy Caller
        await Clients.Caller.SendAsync(
            ChannelNames.ReceiveNewBid,
            groupName,
            newCurrentBid,
            newTotalBids,
            userName,
            newBidTime,
            true
        );

        //Notify others
        await Clients.GroupExcept(groupName.ToString(), Context!.ConnectionId).SendAsync(
            ChannelNames.ReceiveNewBid,
            groupName,                
            newCurrentBid,            
            newTotalBids,              
            userName,             
            newBidTime,
            false
        );        
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // A lógica de remoção do grupo pode ser mais complexa se houver muitos grupos
        // Mas o ASP.NET Core geralmente gerencia a remoção automática na desconexão.
        await base.OnDisconnectedAsync(exception);
    }
}
