using System.Globalization;
using System.Net.Http.Json;
using Application.Interfaces;

namespace Infrastructure.ExternalServices;

public class TelegramService : ITelegramService
{
    private readonly HttpClient _http;
    //private const string PrefixToken = "telegram:token:";
    private string BotToken => "8765187560:AAE6gZ_80ciQ91__BJOoqFivFCzXxALFTJg";
    private string ApiUrl => $"https://api.telegram.org/bot{BotToken}";
    public TelegramService(IHttpClientFactory httpFactory)
    {
        _http = httpFactory.CreateClient("telegram");
    }

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
