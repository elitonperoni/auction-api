using Application.Abstractions.Messaging;
using Application.AuctionUseCases.Create;
using Application.AuctionUseCases.GetDetail;
using Application.AuctionUseCases.List;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace AuctionApi.Endpoints.AuctionBid;

public class List : IEndpoint
{
    public sealed class Request
    {

    }
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("auctions/list", async (
            IQueryHandler<AuctionListQuery, List<AuctionListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AuctionListQuery();

            Result<List<AuctionListResponse>> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auction)
        .RequireAuthorization();
    }
}
