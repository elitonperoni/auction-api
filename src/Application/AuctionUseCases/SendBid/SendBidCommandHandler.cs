using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Auction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SharedKernel;

namespace Application.AuctionUseCases.SendBid;

public class SendBidCommandHandler(
    IApplicationDbContext context
)
    : ICommandHandler<SendBidCommand, SendBidDtoResponse>
{
    public async Task<Result<SendBidDtoResponse>> Handle(SendBidCommand command, CancellationToken cancellationToken)
    {
        using IDbContextTransaction transaction = await context.BeginTransactionAsync(cancellationToken);

        try
        {
            await context.ExecuteSqlRawAsync(
               "SELECT 1 FROM \"auctions\" WHERE \"id\" = @p0 FOR UPDATE",
                new object[] { command.AuctionId },
                cancellationToken);

            decimal currentMax = await context.Bids
                .Where(b => b.AuctionId == command.AuctionId)
                .MaxAsync(b => (decimal?)b.Amount, cancellationToken) ?? 0;

            if (command.BidPrice <= currentMax)
            {
                return Result.Failure<SendBidDtoResponse>(
                    Error.Failure("Bid.Invalid", $"O lance de {command.BidPrice:C} é inferior ao atual {currentMax:C}"));
            }

            Bid auctionBid = new()
            {
                UserId = command.UserId,
                AuctionId = command.AuctionId,
                Amount = command.BidPrice,
                BidDate = DateTime.UtcNow
            };

            await context.Bids.AddAsync(auctionBid, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            int bidCount = await context.Bids
                .CountAsync(b => b.AuctionId == command.AuctionId, cancellationToken);

            return Result.Success(new SendBidDtoResponse
            {
                TotalBids = bidCount,
                Date = auctionBid.BidDate,
                Amount = auctionBid.Amount
            });
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure<SendBidDtoResponse>(
                Error.Failure("Bid.Conflict", "Concorrência detectada: este lance já foi superado."));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure<SendBidDtoResponse>(
                Error.Failure("Bid.Conflict", ex.Message));
            throw;
        }
    }
}
