using System.Net;
using System.Net.Mail;
using Application.Common.Abstractions.Mail;
using Application.Common.Mail;
using Application.Common.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.ExternalServices;

internal sealed class MailSender(IOptions<SecretsApi> options) : IMailSender
{
    private const string SmtpHost = "smtp.gmail.com";
    private const int SmtpPort = 587;

    public async Task SendEmailAsync(SendEmailCommand email, CancellationToken cancellationToken = default)
    {
        SecretsApi settings = options.Value;

        using var smtpClient = new SmtpClient
        {
            Host = SmtpHost,
            Port = SmtpPort,
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(settings.MailAddress, settings.MailPassword)
        };

        using var message = new MailMessage
        {
            From = new MailAddress(settings.MailAddress, settings.MailDisplayName),
            Subject = email.Subject,
            Body = email.Body,
            IsBodyHtml = true
        };

        message.To.Add(email.To);

        await smtpClient.SendMailAsync(message, cancellationToken);
    }
}
