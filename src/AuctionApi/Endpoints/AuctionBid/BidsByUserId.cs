using Application.Abstractions.Messaging;
using Application.AuctionUseCases.BidsByUser;
using Application.AuctionUseCases.ListByUserId;
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
