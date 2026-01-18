using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Auction;
using Domain.Users;
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
        context.ChangeTracker.Clear();
        using IDbContextTransaction transaction = await context.BeginTransactionAsync(cancellationToken);
        try
        {
            await context.ExecuteSqlRawAsync(
               "SELECT 1 FROM \"auctions\" WHERE \"id\" = @p0 FOR UPDATE",
                new object[] { command.AuctionId },
                cancellationToken);

            Auction? auction = await context.Auctions
                .SingleOrDefaultAsync(p => p.Id == command.AuctionId, cancellationToken);

            if (auction is null)
            {
                return Result.Failure<SendBidDtoResponse>(
                    Error.Failure("Bid.Invalid", $"Produto não encontrado"));
            }

            if (command.BidPrice <= auction.CurrentPrice)
            {
                return Result.Failure<SendBidDtoResponse>(
                    Error.Failure("Bid.Invalid", $"O lance de {command.BidPrice:C} é inferior ao atual {auction.CurrentPrice:C}"));
            }

            Bid auctionBid = new()
            {
                UserId = command.UserId,
                AuctionId = command.AuctionId,
                Amount = command.BidPrice,
                BidDate = DateTime.UtcNow                
            };

            auction.CurrentPrice = command.BidPrice;
            auction.LastBidderId = command.UserId;
            auction.BidCount++;
            
            await context.Bids.AddAsync(auctionBid, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            User? user = await context.Users.AsNoTracking().SingleOrDefaultAsync(p => p.Id == command.UserId, cancellationToken);

            return Result.Success(new SendBidDtoResponse
            {
                AuctionId = auctionBid.AuctionId,
                LastBidderId = command.UserId,
                LastBidderNamer = user?.FirstName ?? "",
                TotalBids = auction.BidCount,
                Date = auctionBid.BidDate,
                Amount = auctionBid.Amount
            });
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
