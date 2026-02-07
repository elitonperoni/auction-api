using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Common.Interfaces;
using Application.Enums;
using Application.Extensions;
using Application.Todos.Get;
using Domain.Auction;
using Domain.Interfaces;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.AuctionUseCases.GetDetail;

internal sealed class GetDetailProductQueryHandler(
    IAuctionService auctionService,
    IApplicationDbContext context) 
    : IQueryHandler<GetDetailProductQuery, GetDetailProductResponse>
{
    public async Task<Result<GetDetailProductResponse>> Handle(GetDetailProductQuery query, CancellationToken cancellationToken)
    {
        Auction? auctionDb = await context.Auctions
            .AsNoTracking()
            .Where(auction => auction.Id == query.Id)
            .Include(p => p.User)
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(cancellationToken);        

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


        List<string> photosUrls = [];
        if (auctionDb.Photos != null && auctionDb.Photos.Any())
        {
            photosUrls = auctionService.CreateResponsePhotosUrls(auctionDb.Photos);
        }     

        var response = new GetDetailProductResponse
        {
            Id = auctionDb.Id,
            Title = auctionDb.Title,
            Description = auctionDb.Description,
            CurrentBid = auctionDb.CurrentPrice,
            MinBid = bids.Any() ? Math.Round(bids.Max(p => p?.Amount) * 1.1M ?? 0, 0) : 0,
            BidsCounts = auctionDb.BidCount,
            Category = "Diversos",
            Seller = auctionDb.User?.FirstName ?? "",
            Condition = "Novo",
            Location = "Curitiba, Paraná",
            BidHistory = bids.Any() ? bids.Select(bid => new BidHistoryItem()
            {
                BidderName = bid.User?.FirstName.ToString() ?? "",
                Amount = bid.Amount,
                Date = bid.BidDate
            }).ToList() : [],
            StartDate = auctionDb.StartDate,
            EndDate = auctionDb.EndDate,
            Photos = photosUrls
        };

        return Result.Success(response);
    }
}
