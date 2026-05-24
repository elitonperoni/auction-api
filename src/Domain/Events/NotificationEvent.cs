namespace Domain.Events;

public record NotificationEvent(string CallerId, string Message);
