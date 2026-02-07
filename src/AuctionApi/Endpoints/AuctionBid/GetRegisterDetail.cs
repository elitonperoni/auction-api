using Application.Abstractions.Messaging;
using Application.AuctionUseCases.GetDetail;
using Application.AuctionUseCases.Update;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;

namespace AuctionApi.Endpoints.AuctionBid;

internal sealed class GetRegisterDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)  
    {
        app.MapGet("auctions/register-detail/{id:guid}", async (
            IQueryHandler<GetRegisterDetailQuery, GetRegisterDetailResponse> handler,
            Guid id,
            CancellationToken cancellationToken) =>
        {
            Result<GetRegisterDetailResponse> result =
                await handler.Handle(new GetRegisterDetailQuery(id), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auction)
        .RequireAuthorization();
    }
}

