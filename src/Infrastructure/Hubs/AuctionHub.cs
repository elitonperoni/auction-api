using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Hubs;

public class AuctionHub : Hub
{
    public async Task EntrarNoGrupo(string nomeDoGrupo)
    {
        // 1. Adiciona a conexão atual (Context.ConnectionId) ao grupo.
        // O grupo é criado se ainda não existir.
        await Groups.AddToGroupAsync(Context.ConnectionId, nomeDoGrupo);

        // 2. Opcional: Notificar o usuário que ele entrou no grupo.
        await Clients.Caller.SendAsync("Aviso", $"Você entrou no leilão: {nomeDoGrupo}");

        // 3. Opcional: Notificar os outros membros do grupo que um novo usuário entrou.
        // Clients.OthersInGroup(nomeDoGrupo) envia para todos no grupo, exceto o chamador.
        await Clients.OthersInGroup(nomeDoGrupo).SendAsync("UsuarioEntrouNoGrupo", Context.ConnectionId, nomeDoGrupo);
    }

    // Método que o cliente chamará para sair de um leilão/grupo
    public async Task SairDoGrupo(string nomeDoGrupo)
    {
        // 1. Remove a conexão atual do grupo.
        // Se esta for a última conexão, o grupo é removido.
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, nomeDoGrupo);

        // 2. Opcional: Notificar o usuário que ele saiu do grupo.
        await Clients.Caller.SendAsync("Aviso", $"Você saiu do leilão: {nomeDoGrupo}");

        // 3. Opcional: Notificar os outros membros do grupo que um usuário saiu.
        await Clients.OthersInGroup(nomeDoGrupo).SendAsync("UsuarioSaiuDoGrupo", Context.ConnectionId, nomeDoGrupo);
    }

    // Método que o cliente chamará para enviar uma mensagem/lance para um grupo específico
    public async Task EnviarLanceParaGrupo(string nomeDoGrupo, string usuario, string lance)
    {
        // Envia a mensagem/lance PARA TODOS OS CLIENTES NESTE GRUPO.
        // Use Clients.Group(nomeDoGrupo)
        await Clients.Group(nomeDoGrupo).SendAsync("NovoLance", usuario, lance);
    }

    // O método EnviarMensagem original (agora obsoleto, a menos que você queira broadcast global)
    public async Task EnviarMensagem(string usuario, string mensagem)
    {
        // Este método envia uma mensagem PARA TODOS OS CLIENTES (global broadcast)
        await Clients.All.SendAsync("ReceberMensagem", usuario, mensagem);
    }

    // ... (Métodos OnConnectedAsync e OnDisconnectedAsync permanecem os mesmos) ...
    public override async Task OnConnectedAsync()
    {
        await Clients.Others.SendAsync("UsuarioEntrou", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.Others.SendAsync("UsuarioSaiu", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
