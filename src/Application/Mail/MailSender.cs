using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Mail;
using Application.Mail;
using AuctionApi.Common;
using Microsoft.Extensions.Options;

namespace Infrastructure.Mail;


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
        smtpClient.Send(mailMessage);

        smtpClient.Dispose();
        mailMessage.Dispose();
    }
}
