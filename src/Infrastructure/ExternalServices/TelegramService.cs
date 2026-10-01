using System.Globalization;
using System.Net.Http.Json;
using Application.Common.Interfaces;
using Domain.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.ExternalServices;

internal sealed partial class TelegramService(
    IHttpClientFactory httpFactory,
    IOptions<SecretsApi> options,
    ILogger<TelegramService> logger) : ITelegramService
{
    public const string HttpClientName = "telegram";

    private readonly HttpClient _http = httpFactory.CreateClient(HttpClientName);

    private string ApiUrl => $"https://api.telegram.org/bot{options.Value.ApiKeyTelegram}";

    public Task SendMessageAsync(string chatId, string message, CancellationToken cancellationToken = default)
    {
        if (!long.TryParse(chatId, NumberStyles.Integer, CultureInfo.InvariantCulture, out long parsedChatId))
        {
            LogInvalidChatId(logger, chatId);
            return Task.CompletedTask;
        }

        return PostAsync("sendMessage", new
        {
            chat_id = parsedChatId,
            text = message,
            parse_mode = "HTML"
        }, cancellationToken);
    }

    private async Task PostAsync(string method, object payload, CancellationToken cancellationToken)
    {
        try
        {
            using HttpResponseMessage response = await _http.PostAsJsonAsync($"{ApiUrl}/{method}", payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync(cancellationToken);
                LogRequestFailed(logger, method, (int)response.StatusCode, error);
            }
        }
        catch (HttpRequestException ex)
        {
            LogRequestException(logger, ex, method);
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Invalid Telegram chat id '{ChatId}'")]
    private static partial void LogInvalidChatId(ILogger logger, string chatId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Telegram API call '{Method}' failed with status {StatusCode}: {Error}")]
    private static partial void LogRequestFailed(ILogger logger, string method, int statusCode, string error);

    [LoggerMessage(Level = LogLevel.Error, Message = "Telegram API call '{Method}' threw an exception")]
    private static partial void LogRequestException(ILogger logger, Exception exception, string method);
}
