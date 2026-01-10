using Application.Mail;
using Domain.Configurations;
using Microsoft.Extensions.Options;

namespace Application.Abstractions.Mail;

public interface IMailSender
{
    Task SendEmail(SendEmailCommand requestEmail, IOptions<SecretsApi> options);
}
