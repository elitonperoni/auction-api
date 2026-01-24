using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.AuctionUseCases.GetDetail;
using Application.Common.Interfaces;
using Domain.Auction;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.AuctionUseCases.List;

internal sealed class AutionListQueryHandler(
    IApplicationDbContext context,
    IS3Service s3Service) 
    : IQueryHandler<AuctionListQuery, List<AuctionListResponse>>
{
    public async Task<Result<List<AuctionListResponse>>> Handle(AuctionListQuery query, CancellationToken cancellationToken)
    {
        List<Auction>? auctions = await context.Auctions
            .Include(p => p.Photos)
            .ToListAsync(cancellationToken);

        var response = auctions.Select(p => new AuctionListResponse()
        {
            Id = p.Id,
            CurrentPrice = p.CurrentPrice,
            Title = p.Title,
            BidCount = p.BidCount,
            EndDate = p.EndDate,            
            ImageUrl = p.Photos?.Any() is true
            ? s3Service.GeneratePublicURL($"auction-product-photos/{p.Id}/{ p.Photos?.FirstOrDefault()?.Name}").ToString()
            : ""
        }).ToList();

        return response;
    }
}
