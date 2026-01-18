using Application.Abstractions.Messaging;
using Application.AuctionUseCases.GetDetail;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;

namespace AuctionApi.Endpoints.AuctionBid;

internal sealed class GetDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)  
    {
        app.MapGet("auctions/details/{id:guid}", async (
            IQueryHandler<GetDetailProductQuery, GetDetailProductResponse> handler,
            Guid id,
            CancellationToken cancellationToken) =>
        {
            Result<GetDetailProductResponse> result =
                await handler.Handle(new GetDetailProductQuery(id), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auction)
        .RequireAuthorization();
    }
}

