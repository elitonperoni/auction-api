using Application.Abstractions.Messaging;
using Application.AuctionUseCases.SendBid;
using AuctionApi.Extensions;
using AuctionApi.Hubs;
using AuctionApi.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using SharedKernel;

namespace AuctionApi.Endpoints.AuctionBid;

internal sealed class SendBid : IEndpoint
{
    public sealed class Request
    {
        public Guid UserId { get; set; }
        public decimal Value { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("send", async (
            Request request,
            ICommandHandler<SendBidCommand, SendBidDtoResponse> handler,
            IHubContext<AuctionHub> hubContext,
            CancellationToken cancellationToken) =>
        {
            var command = new SendBidCommand
            {
                UserId = request.UserId,
                BidPrice = request.Value
            };

            Result<SendBidDtoResponse> result = await handler.Handle(command, cancellationToken);
           
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auction);
        //.RequireAuthorization();
    }
}
