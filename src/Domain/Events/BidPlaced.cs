namespace Domain.Events;

public record BidPlaced(
    string CallerId,
    Guid AuctionId,
    Guid UserId,
    decimal Amount,
    DateTime CreatedAt
);
