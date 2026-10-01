using Application.Common.Abstractions.Data;
using Application.Common.Abstractions.Mail;
using Application.Common.Abstractions.Messaging;
using Application.Common.Extensions;
using Application.Common.Mail;
using Application.Common.Options;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Application.Features.Users.Commands.SendRecoveryPassword;

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

        await SendEmailRecoveryPassword(token, user.Email, user.UserName, cancellationToken);

        return true;
    }

    private async Task SendEmailRecoveryPassword(string token, string email, string userName, CancellationToken cancellationToken)
    {
        string linkUrl = $"{options.Value.WebUrl}reset-password?id={token}";

        string htmlBody = $@"
        <html>
            <body>
                <p>Hello {userName}!</p>
                <p>To reset your password, click the link below:</p>
                <p>
                    <a href='{linkUrl}' target='_blank'>Reset my password</a>
                </p>
                <p>Or copy and paste this link into your browser:</p>
                <p>{linkUrl}</p>
            </body>
        </html>";

        await mailSender.SendEmailAsync(
            new SendEmailCommand("Password reset", htmlBody, email),
            cancellationToken);
    }
}
