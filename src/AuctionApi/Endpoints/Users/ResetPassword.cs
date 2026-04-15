using Application.Abstractions.Messaging;
using Application.Users.ResetPassword;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;

namespace AuctionApi.Endpoints.Users;

internal sealed class ResetPassword : IEndpoint
{
    public sealed record Request(string ActualPassword, string NewPassword);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/reset-password", async (
            Request request,
            ICommandHandler<ResetPasswordCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ResetPasswordCommand(request.ActualPassword, request.NewPassword);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .RequireAuthorization();
    }
}
