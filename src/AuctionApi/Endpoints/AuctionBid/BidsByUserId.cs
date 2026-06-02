using Application.Common.Abstractions.Messaging;
using Application.Features.Auctions.Queries.BidsByUser;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;

namespace AuctionApi.Endpoints.AuctionBid;

public class BidsByUserId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("auctions/bids-by-user", async (
            IQueryHandler<AuctionBidsByUserQuery, List<AuctionBidsByUserResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<List<AuctionBidsByUserResponse>> result = await handler.Handle(new AuctionBidsByUserQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auction)
        .RequireAuthorization();
    }
}
