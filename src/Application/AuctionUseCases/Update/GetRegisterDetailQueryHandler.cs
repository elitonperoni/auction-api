using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.AuctionUseCases.Update;

internal sealed class GetRegisterDetailQueryHandler(
    IAuctionService auctionService,
    IApplicationDbContext context)
    : IQueryHandler<GetRegisterDetailQuery, GetRegisterDetailResponse>
{
    public async Task<Result<GetRegisterDetailResponse>> Handle(GetRegisterDetailQuery query, CancellationToken cancellationToken)
    {
        Auction? auctionDb = await context.Auctions
            .AsNoTracking()
            .Where(auction => auction.Id == query.Id)
            .Include(p => p.Photos)
            .Include(p => p.ProductDetail)
            .SingleOrDefaultAsync(cancellationToken);

        if (auctionDb is null)
        {
            return new GetRegisterDetailResponse();
        }

        List<string> photosUrls = [];
        if (auctionDb.Photos != null && auctionDb.Photos.Any())
        {
            photosUrls = auctionService.CreateResponsePhotosUrls(auctionDb.Photos);
        }

        var response = new GetRegisterDetailResponse
        {
            Id = auctionDb.Id,
            Title = auctionDb.Title,
            Description = auctionDb.ProductDetail?.Description ?? "",
            InitialValue = auctionDb.ProductDetail?.StartingPrice ?? 0,
            ConditionProductId = auctionDb.ProductDetail?.ConditionProductId ?? 0,
            CategoryProductId = auctionDb.ProductDetail?.CategoryProductId ?? 0,
            ConditionPackagingId = auctionDb.ProductDetail?.ConditionPackagingId ?? 0,
            WithoutWarranty = auctionDb.ProductDetail?.WithoutWarranty ?? false,
            Country = auctionDb.ProductDetail?.Country ?? "",
            State = auctionDb.ProductDetail?.State ?? "",
            City = auctionDb.ProductDetail?.City ?? "",
            EndDate = auctionDb.EndDate,
            Photos = photosUrls
        };

        return Result.Success(response);
    }
}
