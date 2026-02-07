using System.Globalization;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Common.Interfaces;
using Application.Pagination;
using Domain.Auction;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.AuctionUseCases.List;

internal sealed class AutionListQueryHandler(
    IApplicationDbContext context,
    IS3Service s3Service) 
    : IQueryHandler<AuctionListQuery, PagedResult<AuctionListResponse>>
{
    public async Task<Result<PagedResult<AuctionListResponse>>> Handle(AuctionListQuery query, CancellationToken cancellationToken)
    {
        Pagination<Auction> auctionPagedList = await ApplyFilter(query);
        PaginationMetadata metaDataAuction = auctionPagedList.GetMetadata();

        var response = auctionPagedList.Select(p => new AuctionListResponse()
        {
            Id = p.Id,
            CurrentPrice = p.CurrentPrice,
            Title = p.Title,
            BidCount = p.BidCount,
            EndDate = p.EndDate,
            ImageUrl = p.Photos?.Any() is true
            ? s3Service.BuildPublicUri($"auction-product-photos/{p.Id}/{p.Photos?.FirstOrDefault()?.Name}").ToString()
            : ""
        }).ToList();
        
        return new PagedResult<AuctionListResponse>(response, metaDataAuction);        
    }
    private async Task<Pagination<Auction>> ApplyFilter(AuctionListQuery query)
    {
        IQueryable<Auction> auctionQuery = context.Auctions
                    .Where(p => p.EndDate >= DateTime.UtcNow)
                    .Include(p => p.Photos).AsQueryable();

        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            string searchTerm = $"%{query.SearchTerm.ToLower()}%";

            auctionQuery = auctionQuery.Where(p => EF.Functions.Like(p.Title.ToLower(), searchTerm)
            || EF.Functions.Like(p.Description.ToLower(), searchTerm));
        }

        auctionQuery = auctionQuery.OrderBy(p => p.EndDate);

        Pagination<Auction> pagedAuctions = await PagedList<Auction>.ToPagedList(auctionQuery,
             query.PageIndex,
             query.PageSize);       

        return pagedAuctions;
    }
}
