using Application.Common.Mail;
using Domain.Configurations;
using Microsoft.Extensions.Options;

namespace Application.Common.Abstractions.Mail;

public interface IMailSender
{
    Task SendEmail(SendEmailCommand requestEmail, IOptions<SecretsApi> options);
}
