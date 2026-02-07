using Application.Abstractions.Messaging;
using Application.AuctionUseCases.List;
using Application.AuctionUseCases.ListByUserId;
using Application.Pagination;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;

namespace AuctionApi.Endpoints.AuctionBid;

public class ListByUserId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("auctions/by-userid", async (
            IQueryHandler<AuctionListByUserIdQuery, List<AuctionListByUserIdResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<List<AuctionListByUserIdResponse>> result = await handler.Handle(new AuctionListByUserIdQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auction)
        .RequireAuthorization();
    }
}
