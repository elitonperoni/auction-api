using Application.Abstractions.Messaging;
using Application.Users.Login;
using Application.Users.RefreshToken;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;

namespace AuctionApi.Endpoints.Users;

public class RefreshToken : IEndpoint
{
    public sealed record Request(string Token, string RefreshToken);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/refresh-token", async (
            Request request,
            ICommandHandler<RefreshTokenCommand, RefreshTokenResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RefreshTokenCommand(request.Token, request.RefreshToken);

            Result<RefreshTokenResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .AllowAnonymous();

    }
}
