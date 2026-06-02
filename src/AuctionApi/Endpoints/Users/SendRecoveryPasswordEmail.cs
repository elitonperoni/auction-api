using Application.Common.Abstractions.Messaging;
using Application.Features.Users.Command.SendRecoveryPassword;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;

namespace AuctionApi.Endpoints.Users;

internal sealed class SendRecoveryPasswordEmail : IEndpoint
{
    public sealed record Request(string Email);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/send-recovery-password-email", async (
            Request request,
            ICommandHandler<SendRecoveryPasswordRequestCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new SendRecoveryPasswordRequestCommand(request.Email);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
