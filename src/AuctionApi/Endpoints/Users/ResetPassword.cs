using Application.Abstractions.Messaging;
using Application.Users.RecoveryPassword;
using Application.Users.ResetPassword;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;

namespace AuctionApi.Endpoints.Users;

internal sealed class ResetPassword : IEndpoint
{
    public sealed record Request(string token, string password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/reset-password", async (
            Request request,
            ICommandHandler<ResetPasswordCommand, bool> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ResetPasswordCommand(request.token, request.password);

            Result<bool> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
