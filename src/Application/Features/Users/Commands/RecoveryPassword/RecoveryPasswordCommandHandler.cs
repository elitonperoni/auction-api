using Application.Common.Abstractions.Authentication;
using Application.Common.Abstractions.Data;
using Application.Common.Abstractions.Messaging;
using Domain.Entities;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Users.Commands.RecoveryPassword;

internal sealed class RecoveryPasswordCommandHandler(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher) : ICommandHandler<RecoveryPasswordCommand, bool>
{
    public async Task<Result<bool>> Handle(RecoveryPasswordCommand command, CancellationToken cancellationToken)
    {
        User? user = await context.Users.FirstOrDefaultAsync(u => u.ResetPasswordCode == command.token, cancellationToken);
        if (user == null || user.ResetPasswordExpiry < DateTime.UtcNow || string.IsNullOrEmpty(command.password))
        {
            return Result.Failure<bool>(UserErrors.Unauthorized());
        }

        user.PasswordHash = passwordHasher.Hash(command.password);
        user.ResetPasswordCode = null;
        user.ResetPasswordExpiry = null;

        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
