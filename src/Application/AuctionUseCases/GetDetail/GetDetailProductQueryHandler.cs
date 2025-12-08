using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Todos.Get;
using Domain.Auction;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.AuctionUseCases.GetDetail;

internal sealed class GetDetailProductQueryHandler(
    IApplicationDbContext context) 
    : IQueryHandler<GetDetailProductQuery, GetDetailProductResponse>
{
    public async Task<Result<GetDetailProductResponse>> Handle(GetDetailProductQuery query, CancellationToken cancellationToken)
    {
        Auction? auctionDb = await context.Auctions
            .AsNoTracking()
            .Where(auction => auction.Id == query.Id)
            .Include(p => p.User)
            .FirstOrDefaultAsync(cancellationToken);        

        if (auctionDb is null)
        {
            return new GetDetailProductResponse();
        }    

        List<Bid> bids = await context.Bids
            .AsNoTracking()            
            .Where(bid => bid.AuctionId == auctionDb.Id)
            .Include(p => p.User)
            .OrderByDescending(x => x.BidDate)
            .ToListAsync(cancellationToken);

        var response = new GetDetailProductResponse
        {
            Id = auctionDb.Id,
            Title = auctionDb.Title,
            Description = auctionDb.Description,
            CurrentBid = bids?.Max(p => p.Amount) ?? 0,
            MinBid = bids?.Max(p => p.Amount) * 1.1M ?? 0,
            BidsCounts = bids?.Count ?? 0,
            Category = "Diversos",
            Seller = auctionDb.User?.FirstName ?? "",
            Condition = "Novo",
            Location = "Curitiba, Paraná",
            BidHistory = bids?.Select(bid => new KeyValuePair<string, decimal>(
                bid.User?.FirstName.ToString() ?? "",
                bid.Amount
            )).ToList() ?? new List<KeyValuePair<string, decimal>>(),
            StartDate = auctionDb.StartDate,
            EndDate = auctionDb.EndDate,
            Images = ["images/png"]
        };

        return Result.Success(response);
    }
}
