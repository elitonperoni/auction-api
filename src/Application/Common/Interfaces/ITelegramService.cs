namespace Application.Common.Interfaces;

public interface ITelegramService
{
    /// <summary>
    /// Sends a message to a Telegram chat. Delivery is best-effort: failures are logged and never thrown.
    /// </summary>
    Task SendMessageAsync(string chatId, string message, CancellationToken cancellationToken = default);
}
