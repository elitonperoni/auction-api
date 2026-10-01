using Application.Common.Abstractions.Messaging;
using Application.Features.Users.Commands.Register;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;

namespace AuctionApi.Endpoints.Users;

internal sealed class Register : IEndpoint
{
    public sealed record Request(string Email, string FullName, string UserName,
        string Phone, string Country, string State, string City,
        string Language, string Timezone, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/register", async (
            Request request,
            ICommandHandler<RegisterUserCommand, RegisterUserResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterUserCommand(
                request.Email,
                request.FullName,
                request.UserName,
                request.Phone, 
                request.Country,
                request.State,
                request.City,
                request.Language,
                request.Timezone,
                request.Password);

            Result<RegisterUserResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
