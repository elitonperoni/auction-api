namespace Domain.Events;

public record BidPlaced(
    Guid AuctionId,
    Guid UserId,
    decimal Amount,
    DateTime CreatedAt
);
