using System.Globalization;
using Application.Common.Abstractions.Authentication;
using Application.Common.Abstractions.Data;
using Application.Common.Abstractions.Messaging;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SharedKernel;

namespace Application.Features.Users.Commands.Register;

internal sealed class RegisterUserCommandHandler(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher,
    IStripeService stripeService)
    : ICommandHandler<RegisterUserCommand, RegisterUserResponse>
{
    public async Task<Result<RegisterUserResponse>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(u => u.Email == command.Email, cancellationToken))
        {
            return Result.Failure<RegisterUserResponse>(UserErrors.EmailNotUnique);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            UserName = command.UserName,
            CompleteName = command.FullName,
            Phone = command.Phone,
            PasswordHash = passwordHasher.Hash(command.Password),
            CreatedAt = DateTime.UtcNow,
            Language = int.Parse(command.Language, CultureInfo.InvariantCulture),
            Country = command.Country,
            State = command.State,
            City = command.City,
            TimeZone = command.Timezone
        };

        // The user is only persisted if the checkout session is created: leaving the scope
        // without committing rolls back, so a Stripe failure never leaves an orphan user behind.
        using IDbContextTransaction transaction = await context.BeginTransactionAsync(cancellationToken);

        context.Users.Add(user);

        await context.SaveChangesAsync(cancellationToken);

        Result<StripeCheckoutResponse> checkoutResult =
            await stripeService.CreateSystemAccessCheckoutLinkAsync(
                new StripeSystemAccessCheckoutRequest(
                    user.Id,
                    user.CompleteName,
                    user.Email),
                cancellationToken);

        if (checkoutResult.IsFailure)
        {
            return Result.Failure<RegisterUserResponse>(checkoutResult.Error);
        }

        await transaction.CommitAsync(cancellationToken);

        return Result.Success(new RegisterUserResponse
        {
            UserId = user.Id,
            OrderId = checkoutResult.Value.OrderId,
            CheckoutUrl = checkoutResult.Value.CheckoutUrl
        });
    }
}
