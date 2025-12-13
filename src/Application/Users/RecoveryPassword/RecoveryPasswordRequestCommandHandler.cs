using System.Security.Cryptography;
using Application.Abstractions.Data;
using Application.Abstractions.Mail;
using Application.Abstractions.Messaging;
using Application.Mail;
using AuctionApi.Common;
using AuctionApi.Extensions;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Application.Users.RecoveryPassword;

internal sealed class RecoveryPasswordRequestCommandHandler(
    IApplicationDbContext context,
    IMailSender mailSender,
    IOptions<SecretsApi> options
    ) : ICommandHandler<RecoveryPasswordRequestCommand, string>
{
    public async Task<Result<string>> Handle(RecoveryPasswordRequestCommand command, CancellationToken cancellationToken)
    {
        User? usuario = await context.Users.FirstOrDefaultAsync(u => u.Email == command.email, cancellationToken);
        if (usuario == null)
        {
            return Result.Failure<string>(UserErrors.Unauthorized());
        }

        string token = TokenGenerator.GenerateSecureToken();
        usuario.ResetToken = token;
        usuario.ResetTokenExpiry = DateTime.UtcNow.AddHours(2);

        await context.SaveChangesAsync(cancellationToken);

        await SendEmailRecoveryPassword(token, usuario.Email, usuario.FirstName);

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
