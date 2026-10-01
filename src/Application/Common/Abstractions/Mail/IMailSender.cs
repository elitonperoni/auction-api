using Application.Common.Mail;

namespace Application.Common.Abstractions.Mail;

public interface IMailSender
{
    Task SendEmailAsync(SendEmailCommand email, CancellationToken cancellationToken = default);
}
