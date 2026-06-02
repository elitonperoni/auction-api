using Application.Common.Abstractions.Authentication;
using Application.Common.Abstractions.Data;
using Application.Common.Abstractions.Messaging;
using Domain.Entities;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Users.Command.ResetPassword;

internal sealed class ResetPasswordCommandHandler(IApplicationDbContext context,
    IUserContext userContext,
    IPasswordHasher passwordHasher) : ICommandHandler<ResetPasswordCommand, Guid>
{
    public async Task<Result<Guid>> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;
        User? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(userId));
        }

        bool verified = passwordHasher.Verify(command.ActualPassword, user.PasswordHash);

        if (!verified)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(userId));
        }

        user.PasswordHash = passwordHasher.Hash(command.NewPassword);
        user.ResetPasswordCode = null;
        user.ResetPasswordExpiry = null;

        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
