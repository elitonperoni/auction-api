using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.AuctionUseCases.SendBid;
using Application.Users.Login;
using Domain.Auction;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Users.RefreshToken;

public class RefreshTokenHandler(ITokenProvider tokenProvider,
    IApplicationDbContext context) : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        ClaimsPrincipal refreshResult = tokenProvider.GetPrincipalFromExpiredToken(command.Token);
        Claim? userId = refreshResult.FindFirst(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Result.Failure<RefreshTokenResponse>(Error.Failure("RefreshToken.Invalid", $"Invalid UserId"));
        }

        User? user = await context.Users.SingleOrDefaultAsync(u => u.Id == Guid.Parse(userId.Value), cancellationToken);

        if (user == null || user.RefreshToken != command.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Result.Failure<RefreshTokenResponse>(Error.Failure("RefreshToken.Invalid", $"Invalid refresh token"));
        }

        string newAccessToken = tokenProvider.GenerateAccessToken(user);
        string newRefreshToken = tokenProvider.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new RefreshTokenResponse()
        {
            RefreshToken = newRefreshToken,
            Token = newAccessToken
        });
    }
}
