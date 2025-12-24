using Application.Abstractions.Messaging;
using Application.AuctionUseCases.SendBid;
using Application.Todos.Create;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using SharedKernel;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;

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
            ICommandHandler<SendBidCommand, int> handler,
            IHubContext<AuctionHub> hubContext,
            CancellationToken cancellationToken) =>
        {
            var command = new SendBidCommand
            {
                UserId = request.UserId,
                BidPrice = request.Value
            };

            Result<int> result = await handler.Handle(command, cancellationToken);

            //await hubContext.Clients.All.SendAsync("NovoLance", new
            //{
            //    UserId = Guid.NewGuid(),
            //    Value = 8500
            //}, cancellationToken);


            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auction);
        //.RequireAuthorization();
    }
}
