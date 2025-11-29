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

public class CreateAuctionBidCommandHandler(
    IApplicationDbContext context
    //IDateTimeProvider dateTimeProvider,
    //IUserContext userContext
)
    : ICommandHandler<CreateAuctionBidCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAuctionBidCommand command, CancellationToken cancellationToken)
    {
        Bid auctionBid = new()
        {
            UserId = command.UserId,
            AuctionId = command.AuctionId,
            Amount = command.BidPrice,
            BidDate = DateTime.UtcNow
        };

        await context.Bids.AddAsync(auctionBid, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(auctionBid.Id);
    }
}
