using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Extensions.Entities;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.AuctionUseCases.GetDetail;

internal sealed class GetDetailProductQueryHandler(
    IAuctionService auctionService,
    IUserContext userContext,
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
            .Include(p => p.ProductDetail)
                .ThenInclude(p => p!.CategoryProduct)
            .Include(p => p.ProductDetail)
                .ThenInclude(p => p!.ConditionProduct)
            .Include(p => p.ProductDetail)
                .ThenInclude(p => p!.ConditionPackaging)
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

        Guid currentUserId = userContext.UserId;

        var response = new GetDetailProductResponse
        {
            Id = auctionDb.Id,
            Title = auctionDb.Title,
            Description = auctionDb.ProductDetail?.Description ?? "",
            CurrentBid = auctionDb.CurrentPrice,
            MinBid = bids.Any() ? Math.Round(bids.Max(p => p.Amount) * 1.01m, 2) : 0,
            BidsCounts = auctionDb.BidCount,            
            IsOwner = auctionDb.User?.Id == currentUserId,
            Seller = auctionDb.User?.UserName ?? "",
            Category = auctionDb.ProductDetail?.CategoryProduct?.Name ?? "",
            ConditionProduct = auctionDb.ProductDetail?.ConditionProduct?.Name ?? "",
            ConditionPackaging = auctionDb.ProductDetail?.ConditionPackaging?.Name ?? "",
            WithoutWarranty = auctionDb.ProductDetail?.WithoutWarranty ?? false,
            Location = auctionDb.ProductDetail?.GetAddress() ?? "",
            BidHistory = bids.Any() ? bids.Select(bid => new BidHistoryItem()
            {
                BidderName = bid.User?.UserName.ToString() ?? "",
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
