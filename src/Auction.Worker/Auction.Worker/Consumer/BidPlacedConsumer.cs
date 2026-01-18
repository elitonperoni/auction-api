using Application.Abstractions.Messaging;
using Application.AuctionUseCases.SendBid;
using Domain.Events;
using MassTransit;
using SharedKernel;

namespace Auction.Worker.Consumer;

public class BidPlacedConsumer(ICommandHandler<SendBidCommand, SendBidDtoResponse> handler,
    ILogger<BidPlacedConsumer> logger,
    IPublishEndpoint publishEndpoint) : 
    IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        string workerId = Environment.MachineName;
        decimal bidAmount = context.Message.Amount;
        Guid auctionId = context.Message.AuctionId;

        logger.LogInformation("[Worker:{Id}] Processando lance de {Valor} para o leilão {AuctionId}",
                workerId, bidAmount, auctionId);

        var command = new SendBidCommand()
        {
            AuctionId = context.Message.AuctionId,
            BidPrice = context.Message.Amount,
            UserId = context.Message.UserId,
        };

        Result<SendBidDtoResponse> responseBid = await handler.Handle(command, context.CancellationToken);

        if (responseBid.IsFailure)
        {
            logger.LogWarning("[Worker:{Id}] LANCE REJEITADO: Valor {Valor} | Motivo: {Reason}",
                workerId, bidAmount, responseBid.Error.Description);
            return;
        }

        await publishEndpoint.Publish(new BidProcessedEvent(
            responseBid.Value.AuctionId,
            responseBid.Value.Amount,
            responseBid.Value.TotalBids,
            responseBid.Value.LastBidderId,
            responseBid.Value.LastBidderNamer,
            responseBid.Value.Date),
            context.CancellationToken);
    }
}
