using Application.Common.Abstractions.Authentication;
using Application.Common.Abstractions.Data;
using Application.Common.Abstractions.Messaging;
using Application.Common.Enums;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Auctions.Queries.ListByUserId;


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
            Title = p.Title ?? string.Empty,
            BidCount = p.BidCount,
            EndDate = p.EndDate,
            ImageUrl = p.Photos?.Any() is true
            ? s3Service.BuildPublicUri($"{AWSS3Folder.AuctionProductPhotos.GetDescription()}/{p.Id}/{p.Photos?.FirstOrDefault()?.Name}").ToString()
            : "",
            ActualWinner = !string.IsNullOrEmpty(p.LastBidder?.UserName) ? $"@{p.LastBidder?.UserName}" : null,
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
