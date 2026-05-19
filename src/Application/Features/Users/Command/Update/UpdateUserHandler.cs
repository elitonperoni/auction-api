using Application.Common.Abstractions.Authentication;
using Application.Common.Abstractions.Data;
using Application.Common.Abstractions.Messaging;
using Application.Common.Extensions.Entities;
using Domain.Entities;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Users.Command.Update;

internal sealed class UpdateUserHandler(IApplicationDbContext context,
    IUserContext userContext) : ICommandHandler<UpdateUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        if (userContext.UserId != command.UserId)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(command.UserId));
        }

        User? user = await context.Users.SingleOrDefaultAsync(p => p.Id == command.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(command.UserId));
        }

        user.UpdateData(command);

        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }
}
