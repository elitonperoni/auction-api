using System.Globalization;
using System.Net.Http.Json;
using Application.Common.Interfaces;
using Domain.Configurations;
using Microsoft.Extensions.Options;

namespace Infrastructure.ExternalServices;

public class TelegramService(IHttpClientFactory httpFactory, IOptions<SecretsApi> options) : ITelegramService
{
    private readonly HttpClient _http = httpFactory.CreateClient("telegram");
    private string ApiUrl => $"https://api.telegram.org/bot{options.Value.ApiKeyTelegram}";
    public async Task SendMessage(string chatId, string mensagem)
    {
        await PostAsync("sendMessage", new
        {
            chat_id = long.Parse(chatId, CultureInfo.InvariantCulture),
            text = mensagem,
            parse_mode = "HTML"
        });
    }

    private async Task PostAsync(string metodo, object payload)
    {
        try
        {
            HttpResponseMessage response = await _http.PostAsJsonAsync($"{ApiUrl}/{metodo}", payload);

            if (!response.IsSuccessStatusCode)
            {
                string erro = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[Telegram] Erro ao chamar {metodo}: {erro}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Telegram] Exceção: {ex.Message}");
        }
    }
}
