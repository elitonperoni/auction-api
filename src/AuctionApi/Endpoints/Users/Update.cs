using Application.Abstractions.Messaging;
using Application.Users.Update;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;

namespace AuctionApi.Endpoints.Users;

internal sealed class Update : IEndpoint
{
    public sealed record Request(string Name, string Email, string UserName, string Phone, string State, string City, string Country, int Language,  string TimeZone);    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/update/{userId:guid}", async (
           Request request,
           Guid userId,           
           ICommandHandler<UpdateUserCommand, Guid> handler,
           CancellationToken cancellationToken) =>
        {
            var command = new UpdateUserCommand
            {
                UserId = userId,
                Name = request.Name,
                Email = request.Email,
                UserName = request.UserName,
                Phone = request.Phone,
                State = request.State,
                City = request.City,
                Country = request.Country,
                Language = request.Language,
                TimeZone = request.TimeZone
            };

        Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
       .WithTags(Tags.Users)
       .RequireAuthorization();
    }
}
