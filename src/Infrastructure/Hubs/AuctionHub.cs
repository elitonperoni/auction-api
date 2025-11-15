using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.AuctionUseCases.Create;
using Application.AuctionUseCases.SendBid;
using Application.Extensions;
using Application.Todos.Complete;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Hubs;

[Authorize]
public class AuctionHub(ICommandHandler<CreateAuctionBidCommand, Guid> handler) : Hub
{
    public async Task JoinAuctionGroup(string groupName)
    {
        // 1. Adiciona a conexão atual (o cliente) ao grupo específico do produto.
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        //_logger.LogInformation($"Cliente {Context.ConnectionId} adicionado ao grupo {productGroupName}");

        // Opcional: Enviar uma mensagem de boas-vindas ou o estado atual do leilão
        await Clients.Caller.SendAsync("ReceiveMessage", "AuctionHub", $"Você entrou no leilão: {groupName}");
    }
    public async Task SendBid(string groupName, string bidValueString)
    {
        //Guid userId = Guid.Parse(Context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        string userName =  Context?.User?.Claims
            .FirstOrDefault(c => c.Type == "name")?
            .Value ?? "Usuário anônimo";

        //if (string.IsNullOrEmpty(userId))
        //{
        //    await Clients.Caller.SendAsync("BidError", "Usuário não autenticado.");
        //    return;
        //}

        if (!decimal.TryParse(bidValueString, out decimal bidAmount))
        {
            await Clients.Caller.SendAsync("BidError", "Valor do lance inválido.");
            return;
        }
        await handler.Handle(new CreateAuctionBidCommand
        {
            AuctionId = Guid.Parse(groupName), //auctionId,
            UserId = Guid.Parse("964d11f5-ced2-488f-bce2-d3e80e6c0693"), //userId,
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
