using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Extensions;
using Application.Users.Login;
using Domain.Entities;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Users.ResetPassword;

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
