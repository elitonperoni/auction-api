namespace Application.Common.Mail;

public record SendEmailCommand(string Subject, string Body, string To);
