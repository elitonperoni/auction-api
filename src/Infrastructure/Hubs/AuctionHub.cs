using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.AuctionUseCases.Create;
using Application.AuctionUseCases.SendBid;
using Application.Todos.Complete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Hubs;


// Assuma que você tem um serviço para interagir com o banco de dados/lógica de leilão
// Exemplo: ILeilaoService
//[Authorize]
public class AuctionHub : Hub
{
    private readonly ICommandHandler<CreateAuctionBidCommand, Guid> _handler;

    // O Handler é injetado, pois ele está na Camada de Aplicação
    public AuctionHub(ICommandHandler<CreateAuctionBidCommand, Guid> handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Método chamado quando o cliente Next.js se conecta.
    /// Ele deve chamar Clients.Group(productGroupName).SendAsync.
    /// </summary>
    /// <param name="groupName">O ID do produto (ex: "product_1") que o cliente está visualizando.</param>
    public async Task JoinAuctionGroup(string groupName)
    {
        // 1. Adiciona a conexão atual (o cliente) ao grupo específico do produto.
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        //_logger.LogInformation($"Cliente {Context.ConnectionId} adicionado ao grupo {productGroupName}");

        // Opcional: Enviar uma mensagem de boas-vindas ou o estado atual do leilão
        await Clients.Caller.SendAsync("ReceiveMessage", "AuctionHub", $"Você entrou no leilão: {groupName}");
    }


    /// <summary>
    /// Recebe o novo lance do cliente Next.js (via invoke).
    /// Assinatura do cliente Next.js: connection.invoke("SendBid", groupName, user, bidAmount.toString())
    /// </summary>
    /// <param name="productGroupName">O grupo/ID do produto (ex: "product_1").</param>
    /// <param name="user">O ID ou nome do usuário que fez o lance.</param>
    /// <param name="bidValueString">O valor do lance como string.</param>
    public async Task SendBid(string productGroupName, string bidValueString)
    {
        var userId = Guid.Parse(Context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        string userName = Context?.User?.Identity?.Name ?? "Licitante Desconhecido";

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

        // --- 1. Lógica de Processamento do Lance (Simulação) ---
        // Na vida real, você chamaria _leilaoService.ProcessBid(productGroupName, user, bidAmount);
        var productId = Guid.Parse(productGroupName.Replace("product_", ""), System.Globalization.CultureInfo.InvariantCulture);

        await _handler.Handle(new CreateAuctionBidCommand
        {
            UserId = userId,
            BidPrice = bidAmount,
        }, CancellationToken.None);


        // Simulação de Obtenção de Dados Pós-Lance:
        // Na realidade, esses valores viriam do seu serviço/banco de dados após um lance bem-sucedido.
        decimal newCurrentBid = bidAmount;
        int newTotalBids = 20 + 1;
        string newBidderName = userName;
        string newBidTime = DateTime.Now.ToString("HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        // --- 2. Broadcast para os Clientes Next.js ---

        // CONTRATO: SendAsync("SendBid", receivedProductId, newBidAmount, newTotalBids, newBidderName, newBidTime)

        await Clients.Group(productGroupName).SendAsync(
            "SendBid",                 // Nome do método no cliente (newConnection.on("SendBid", ...))
            productId,                 // 1. ID do Produto (number)
            newCurrentBid,             // 2. Novo Valor do Lance (number)
            newTotalBids,              // 3. Novo Total de Lances (number)
            newBidderName,             // 4. Nome do Licitante (string)
            newBidTime                 // 5. Tempo do Lance (string)
        );

        //_logger.LogInformation($"Lance de {newCurrentBid} enviado para o grupo {productGroupName} por {user}.");
    
    
    }

    // Opcional: Remover do grupo quando o cliente desconectar (ou fechar a aba)
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // A lógica de remoção do grupo pode ser mais complexa se houver muitos grupos
        // Mas o ASP.NET Core geralmente gerencia a remoção automática na desconexão.
        await base.OnDisconnectedAsync(exception);
    }
}
