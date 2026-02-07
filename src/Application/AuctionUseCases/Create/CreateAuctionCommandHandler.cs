using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Common.Interfaces;
using Application.Enums;
using Application.Extensions;
using Domain.Auction;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.AuctionUseCases.Create;
public class CreateAuctionCommandHandler(
    IApplicationDbContext context,
    IS3Service s3Service,
    IUserContext userContext
    )
    : ICommandHandler<CreateAuctionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAuctionCommand command, CancellationToken cancellationToken)
    {        
        Guid auctionId = command.Id.HasValue && command.Id != Guid.Empty
            ? await UpdateAuction(command, cancellationToken)
            : await SaveNewAuction(command, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(auctionId);
    }
    private async Task<Guid> SaveNewAuction(CreateAuctionCommand command, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        Auction auction = new()
        {
            Title = command.Title,
            Description = command.Description,
            StartDate = DateTime.UtcNow,
            EndDate = command.EndDate,
            StartingPrice = command.StartingPrice,
            CurrentPrice = command.StartingPrice,
            UserId = userId,
        };

        await context.Auctions.AddAsync(auction, cancellationToken);

        if (command.NewImages.Any())
        {
            await SavePhotos(command, auction.Id);
        }

        return auction.Id;
    }

    private async Task<Guid> UpdateAuction(CreateAuctionCommand command, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        Auction? auction = await context.Auctions
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(c => c.Id == command.Id && c.UserId == userId, cancellationToken);

        if (auction is null)
        {
            return Guid.Empty;
        }

        auction.Title = command.Title;
        auction.Description = command.Description;
        auction.EndDate = command.EndDate;

        if (command.ImagesToRemove?.Any() is true)
        {
            await RemovePhotos(command, auction);
        }

        if (command.NewImages.Any())
        {
            await SavePhotos(command, auction.Id);
        }

        context.Auctions.Update(auction);

        return auction.Id;
    }

    private async Task RemovePhotos(CreateAuctionCommand command, Auction auction)
    {
        if (command?.ImagesToRemove is null || !command.ImagesToRemove.Any())
        {
            return;
        }            

        foreach (string item in command.ImagesToRemove)
        {
            var uri = new Uri(item);
            string fileName = Path.GetFileName(uri.AbsolutePath);

            ProductPhoto? photoEntity = auction.Photos?.FirstOrDefault(p => p.Name == fileName);

            if (photoEntity is not null)
            {
                auction.Photos?.Remove(photoEntity);
                await s3Service.DeleteFile(photoEntity.Name);
            }
        }
    }

    private async Task SavePhotos(CreateAuctionCommand command, Guid auctionId)
    {
        if (command.NewImages.Any())
        {
            List<ProductPhoto> newImages = [];
            foreach (FileInput item in command.NewImages)
            {
                var idPhoto = Guid.NewGuid();
                string contentType = item.ContentType.Replace("image/", "");
                string fileName = $"{idPhoto}.{contentType}";

                newImages.Add(new ProductPhoto
                {
                    Id = idPhoto,
                    Name = fileName,
                    ContentType = contentType,
                    AuctionId = auctionId
                });

                await s3Service.UploadImageAsync(item.Stream,
                    $"{AWSS3Folder.AuctionProductPhotos.GetDescription()}/{auctionId}",
                    fileName, contentType);
            }

            context.ProductPhotos.AddRange(newImages);
        }
    }
}

