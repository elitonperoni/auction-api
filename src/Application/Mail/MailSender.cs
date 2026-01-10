using System.Net;
using System.Net.Mail;
using Application.Abstractions.Mail;
using Domain.Configurations;
using Microsoft.Extensions.Options;

namespace Application.Mail;


public sealed class MailSender : IMailSender
{
    public async Task SendEmail(SendEmailCommand requestEmail, IOptions<SecretsApi> options)
    {
        MailMessage mailMessage = new();
        var smtpClient = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(options.Value.MailAddress, options.Value.MailPassword)
        };
        mailMessage.From = new MailAddress(options.Value.MailAddress, "Leilão Max");
        mailMessage.Body = requestEmail.Body;
        mailMessage.Subject = requestEmail.Subject;
        mailMessage.IsBodyHtml = true;
        mailMessage.To.Add(requestEmail.To);
        await smtpClient.SendMailAsync(mailMessage);

        smtpClient.Dispose();
        mailMessage.Dispose();
    }
}
