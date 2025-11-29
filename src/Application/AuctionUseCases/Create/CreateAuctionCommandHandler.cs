using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Auction;
using SharedKernel;

namespace Application.AuctionUseCases.Create;
public class CreateAuctionCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<CreateAuctionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAuctionCommand command, CancellationToken cancellationToken)
    {
        Auction auction = new()
        {
            Title = command.Title,
            Description = command.Description,
            StartDate = command.StartDate,
            EndDate = DateTime.UtcNow.AddHours(24),
            StartingPrice = command.StartingPrice,
            UserId = command.UserId
        };

        await context.Auctions.AddAsync(auction, cancellationToken);

        return Result.Success(auction.Id);
    }    
}

