using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Common.Interfaces;
using Domain.Auction;
using SharedKernel;

namespace Application.AuctionUseCases.Create;
public class CreateAuctionCommandHandler(
    IApplicationDbContext context,
    //IS3Service s3Service,
    IUserContext userContext
    )
    : ICommandHandler<CreateAuctionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAuctionCommand command, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        //string fileName = $"{Guid.NewGuid().ToString()}";

        //await s3Service.UploadImageAsync(command.ImageStream, "images", fileName, command.ContentType);

        Auction auction = new()
        {
            Title = command.Title,
            Description = command.Description,
            StartDate = command.StartDate,
            EndDate = DateTime.UtcNow.AddHours(24),
            StartingPrice = command.StartingPrice,
            UserId = userId
        };

        await context.Auctions.AddAsync(auction, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(auction.Id);
    }    
}

