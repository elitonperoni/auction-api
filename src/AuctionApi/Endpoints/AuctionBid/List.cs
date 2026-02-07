using Application.Abstractions.Messaging;
using Application.AuctionUseCases.Create;
using Application.AuctionUseCases.GetDetail;
using Application.AuctionUseCases.List;
using Application.Pagination;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace AuctionApi.Endpoints.AuctionBid;

public class List : IEndpoint
{    
    public record Request : PaginationParams
    {
        public string? SearchTerm { get; set; }
    }
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("auctions/list", async (
            [AsParameters] Request request,
            IQueryHandler <AuctionListQuery, PagedResult<AuctionListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AuctionListQuery(request.SearchTerm)
            {                
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            Result<PagedResult<AuctionListResponse>> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auction);
    }
}
