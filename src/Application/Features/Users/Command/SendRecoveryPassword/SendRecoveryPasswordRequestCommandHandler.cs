using Application.Common.Abstractions.Data;
using Application.Common.Abstractions.Mail;
using Application.Common.Abstractions.Messaging;
using Application.Common.Extensions;
using Application.Common.Mail;
using Domain.Configurations;
using Domain.Entities;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Application.Features.Users.Command.SendRecoveryPassword;

internal sealed class SendRecoveryPasswordRequestCommandHandler(
    IApplicationDbContext context,
    IMailSender mailSender,
    IOptions<SecretsApi> options
    ) : ICommandHandler<SendRecoveryPasswordRequestCommand, string>
{
    public async Task<Result<string>> Handle(SendRecoveryPasswordRequestCommand command, CancellationToken cancellationToken)
    {
        User? usuario = await context.Users.FirstOrDefaultAsync(u => u.Email == command.email, cancellationToken);
        if (usuario == null)
        {
            return Result.Failure<string>(UserErrors.Unauthorized());
        }

        string token = TokenGenerator.GenerateSecureToken();
        usuario.ResetPasswordCode = token;
        usuario.ResetPasswordExpiry = DateTime.UtcNow.AddHours(2);

        await context.SaveChangesAsync(cancellationToken);

        await SendEmailRecoveryPassword(token, usuario.Email, usuario.UserName);

        return Result.Success(token); 
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
