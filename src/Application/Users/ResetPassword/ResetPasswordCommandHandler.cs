using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Users.ResetPassword;

internal sealed class ResetPasswordCommandHandler(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher) : ICommandHandler<ResetPasswordCommand, bool>
{
    public async Task<Result<bool>> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        User? usuario = await context.Users.FirstOrDefaultAsync(u => u.ResetToken == command.token, cancellationToken);
        if (usuario == null || usuario.ResetTokenExpiry < DateTime.UtcNow || string.IsNullOrEmpty(command.password))
        {
            return Result.Failure<bool>(UserErrors.Unauthorized());
        }

        usuario.PasswordHash = passwordHasher.Hash(command.password);
        usuario.ResetToken = null;
        usuario.ResetTokenExpiry = null;

        context.Users.Update(usuario);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
