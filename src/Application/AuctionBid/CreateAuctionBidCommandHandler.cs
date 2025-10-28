using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Auction;
using Application.Common.Interfaces;
using Application.Todos.Complete;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Bid;

public class CreateAuctionBidCommandHandler(
    IAuctionNotifier auctionNotifier
    //IApplicationDbContext context,
    //IDateTimeProvider dateTimeProvider,
    //IUserContext userContext
)
    : ICommandHandler<CreateAuctionBidCommand>
{
    public async Task<Result> Handle(CreateAuctionBidCommand command, CancellationToken cancellationToken)
    {
        // Implementation for creating a bid

        await auctionNotifier.NotifyNewBid("auctionId", "bidderId");

        return Result.Success();
    }
}
