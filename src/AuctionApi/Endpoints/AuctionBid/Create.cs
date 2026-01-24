using Application.Abstractions.Messaging;
using Application.AuctionUseCases.Create;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace AuctionApi.Endpoints.AuctionBid;

public class Create : IEndpoint
{
    public sealed class Request
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EndDate { get; set; }
        public decimal StartingPrice { get; set; }
        public IFormFileCollection Images { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auctions/new", async (
            [FromForm] Request request,
            ICommandHandler<CreateAuctionCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var imageModels = request.Images.Select(file =>
                new FileInput(
                    file.OpenReadStream(),
                    file.FileName,
                    file.ContentType
                )).ToList();

            var command = new CreateAuctionCommand
            {
                Title = request.Title,
                Description = request.Description,
                EndDate = request.EndDate,
                StartingPrice = request.StartingPrice,
                ImageStreams = imageModels,
            };

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auction)
        .DisableAntiforgery()
        .RequireAuthorization();
    }
}
