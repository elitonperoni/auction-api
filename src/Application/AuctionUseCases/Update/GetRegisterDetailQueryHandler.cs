using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.AuctionUseCases.GetDetail;
using Application.Common.Interfaces;
using Application.Enums;
using Application.Extensions;
using Domain.Auction;
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
            Description = auctionDb.Description,
            InitialValue = auctionDb.StartingPrice,
            EndDate = auctionDb.EndDate,
            Photos = photosUrls
        };

        return Result.Success(response);
    }
}
