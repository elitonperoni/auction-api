namespace Application.Common.Options;

/// <summary>Bound to the "SecretsApi" configuration section.</summary>
public sealed class SecretsApi
{
    public string MailAddress { get; set; } = string.Empty;
    public string MailPassword { get; set; } = string.Empty;
    public string MailDisplayName { get; set; } = "Auction";
    public string WebUrl { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string ApiKeyTelegram { get; set; } = string.Empty;
    public string TelegramBotUsername { get; set; } = "auctionmax_bot";
    public string TelegramWebhookSecret { get; set; } = string.Empty;
}
