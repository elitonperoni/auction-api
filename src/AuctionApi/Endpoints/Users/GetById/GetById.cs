using SharedKernel;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using Application.Features.Users.Queries.GetById;
using Application.Common.Abstractions.Messaging;

namespace AuctionApi.Endpoints.Users.GetById;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{userId}", async (
            Guid userId,
            IQueryHandler<GetUserByIdQuery, UserResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserByIdQuery(userId);

            Result<UserResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(Permissions.UsersAccess)
        .WithTags(Tags.Users);
    }
}
