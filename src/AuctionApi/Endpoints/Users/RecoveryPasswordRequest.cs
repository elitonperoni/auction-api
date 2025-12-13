using Application.Abstractions.Messaging;
using Application.Users.RecoveryPassword;
using Application.Users.Register;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;

namespace AuctionApi.Endpoints.Users;

internal sealed class RecoveryPasswordRequest : IEndpoint
{
    public sealed record Request(string Email);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/recovery-password", async (
            Request request,
            ICommandHandler<RecoveryPasswordRequestCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RecoveryPasswordRequestCommand(request.Email);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
