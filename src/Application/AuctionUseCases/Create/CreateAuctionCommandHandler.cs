using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Common.Interfaces;
using Domain.Auction;
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

        if (command.ImageStreams.Any())
        {
            auction.Photos = [];

            foreach (FileInput item in command.ImageStreams)
            {
                var idPhoto = Guid.NewGuid();
                string contentType = item.ContentType.Replace("image/", "");
                string fileName = $"{idPhoto}.{contentType}";

                ProductPhoto photo = new()
                {
                    Id = idPhoto,
                    Name = fileName,
                    ContentType = contentType
                };

                auction.Photos.Add(photo);

                await s3Service.UploadImageAsync(item.Stream, $"auction-product-photos/{auction.Id}", fileName, contentType);
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(auction.Id);
    }    
}

