using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Common.Interfaces;
using Application.Todos.Complete;
using Domain.Auction;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.AuctionUseCases.SendBid;

public class SendBidCommandHandler(
    IApplicationDbContext context
    //IDateTimeProvider dateTimeProvider,
    //IUserContext userContext
)
    : ICommandHandler<SendBidCommand, int>
{
    public async Task<Result<int>> Handle(SendBidCommand command, CancellationToken cancellationToken)
    {
        if (!IsValidBid(command.AuctionId, command.BidPrice))
        {
            return Result.Failure<int>(Error.Failure("TodoItems.AlreadyCompleted", "Lance enviado é inferior ao mínimo"));
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

        int bidCount = await context.Bids.CountAsync(b => b.AuctionId == command.AuctionId, cancellationToken);

        return Result.Success(bidCount);
    }

    private bool IsValidBid(Guid auctionId, decimal bidPrice)
    {
        decimal maxAmount = context.Bids.Where(x => x.AuctionId == auctionId).Max(p => p.Amount);

        return bidPrice > maxAmount;
    }
}
