namespace Domain.Events;

public record BidProcessedEvent(
    Guid AuctionId, 
    decimal TotalAmount, 
    int TotalBids, 
    Guid LastBidderId, 
    string LastBidderName, 
    Guid AuctionOwnerId,
    string MessageToOwner,
    string DescriptionDetail,
    DateTime ProcessedAt);
