using Application.Common.Abstractions.Data;
using Application.Common.Abstractions.Mail;
using Application.Common.Abstractions.Messaging;
using Application.Common.Extensions;
using Application.Common.Mail;
using Domain.Configurations;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Application.Features.Users.Command.SendRecoveryPassword;

internal sealed class SendRecoveryPasswordRequestCommandHandler(
    IApplicationDbContext context,
    IMailSender mailSender,
    IOptions<SecretsApi> options
    ) : ICommandHandler<SendRecoveryPasswordRequestCommand, bool>
{
    // Always reports success so the endpoint cannot be used to discover registered emails,
    // and never returns the reset token: it is only delivered by email.
    public async Task<Result<bool>> Handle(SendRecoveryPasswordRequestCommand command, CancellationToken cancellationToken)
    {
        User? user = await context.Users.FirstOrDefaultAsync(u => u.Email == command.email, cancellationToken);
        if (user is null)
        {
            return true;
        }

        string token = TokenGenerator.GenerateSecureToken();
        user.ResetPasswordCode = token;
        user.ResetPasswordExpiry = DateTime.UtcNow.AddHours(2);

        await context.SaveChangesAsync(cancellationToken);

        await SendEmailRecoveryPassword(token, user.Email, user.UserName);

        return true;
    }

    private async Task SendEmailRecoveryPassword(string token, string email, string userName)
    {
        string linkUrl = $"{options.Value.WebUrl}reset-password?id={token}";

        string htmlBody = $@"
        <html>
            <body>
                <p>Olá {userName}!</p>
                <p>Para redefinir sua senha, clique no link abaixo:</p>
                <p>
                    <a href='{linkUrl}' target='_blank'>Redefinir minha senha</a>
                </p>
                <p>Ou copie e cole o link abaixo no seu navegador:</p>
                <p>{linkUrl}</p>
            </body>
        </html>";

        await mailSender.SendEmail(new SendEmailCommand(
            "Redefinicação de senha - Leilão Max",
            htmlBody,
            email
            ), options);
    }
}
