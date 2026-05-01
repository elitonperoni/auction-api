namespace Application.Interfaces;

public interface ITelegramService
{
    Task SendMessage(string chatId, string mensagem);
}
