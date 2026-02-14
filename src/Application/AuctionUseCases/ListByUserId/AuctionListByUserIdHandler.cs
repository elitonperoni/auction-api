using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.AuctionUseCases.List;
using Application.Interfaces;
using Application.Pagination;
using Domain.Auction;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.AuctionUseCases.ListByUserId;


internal sealed class AuctionListByUserIdHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IS3Service s3Service)
    : IQueryHandler<AuctionListByUserIdQuery, List<AuctionListByUserIdResponse>>
{
    public async Task<Result<List<AuctionListByUserIdResponse>>> Handle(AuctionListByUserIdQuery query, CancellationToken cancellationToken)
    {
        List<Auction> auctionsByUserId = await GetAuctionsByUserId(cancellationToken);

        var response = auctionsByUserId.Select(p => new AuctionListByUserIdResponse()
        {
            Id = p.Id,
            CurrentPrice = p.CurrentPrice,
            Title = p.Title,
            BidCount = p.BidCount,
            EndDate = p.EndDate,
            ImageUrl = p.Photos?.Any() is true
            ? s3Service.BuildPublicUri($"auction-product-photos/{p.Id}/{p.Photos?.FirstOrDefault()?.Name}").ToString()
            : "",
            ActualWinner = !string.IsNullOrEmpty(p.LastBidder?.FirstName) ? $"@{p.LastBidder?.FirstName}" : null,
            Status = p.EndDate > DateTime.UtcNow ? "Ativo" : "Finalizado"
        }).ToList();

        return response;
    }
    private async Task<List<Auction>> GetAuctionsByUserId(CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        return await context.Auctions
                    .OrderByDescending(p => p.EndDate)
                    .Where(p => p.UserId == userId)
                    .Include(p => p.Photos)
                    .Include(p => p.LastBidder)
                    .ToListAsync(cancellationToken);       
    }
}
