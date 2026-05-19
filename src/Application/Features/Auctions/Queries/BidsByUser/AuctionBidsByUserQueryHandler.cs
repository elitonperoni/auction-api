using Application.Common.Abstractions.Authentication;
using Application.Common.Abstractions.Data;
using Application.Common.Abstractions.Messaging;
using Application.Common.Enums;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Auctions.Queries.BidsByUser;

internal sealed class AuctionBidsByUserQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IS3Service s3Service)
    : IQueryHandler<AuctionBidsByUserQuery, List<AuctionBidsByUserResponse>>
{
    public async Task<Result<List<AuctionBidsByUserResponse>>> Handle(AuctionBidsByUserQuery query, CancellationToken cancellationToken)
    {
        List<Auction> auctionsByUserId = await GetAuctionsByUserId(cancellationToken);

        Guid userId = userContext.UserId;

        var response = auctionsByUserId.Select(p => new AuctionBidsByUserResponse()
        {
            Id = p.Id,
            CurrentPrice = p.CurrentPrice,
            UserLastBidAmount = p.Bids?.Max(p => p.Amount) ?? 0,
            Title = p.Title ?? "",
            BidCount = p.BidCount,
            EndDate = p.EndDate,
            ImageUrl = p.Photos?.Any() is true
            ? s3Service.BuildPublicUri($"{AWSS3Folder.AuctionProductPhotos.GetDescription()}/{p.Id}/{p.Photos?.FirstOrDefault()?.Name}").ToString()
            : "",
            ActualLeader = p.LastBidder?.UserName ?? "",
            IsUserActualLeader = p.LastBidder?.Id == userId,
            IsUserWinner = p.LastBidder?.Id == userId && p.EndDate < DateTime.UtcNow,
            UserBidsCount = p.Bids?.Count ?? 0,
        }).ToList();

        return response;
    }
    private async Task<List<Auction>> GetAuctionsByUserId(CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        return await context.Auctions
                    .AsNoTracking()
                    .AsSplitQuery()
                    .OrderByDescending(p => p.EndDate)
                    .Where(p => p.Bids != null && p.Bids.Any(p => p.UserId == userId))
                    .Include(p => p.Photos)
                    .Include(p => p.Bids!.Where(p => p.UserId == userId))
                    .Include(p => p.LastBidder)
                    .ToListAsync(cancellationToken);
    }
}
