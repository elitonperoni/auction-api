namespace Application.Features.Auctions.Commands.SendBid;

public sealed class SendBidDtoResponse
{
    public Guid AuctionId { get; set; }
    public int TotalBids { get; set; }
    public Guid LastBidderId { get; set; }
    public string LastBidderNamer { get; set; }
    public Guid AuctionOwnerId { get; set; }
    public string MessageToOwner { get; set; }
    public string DescriptionDetail { get; set; }
    public DateTime Date { get; set; }    
    public decimal Amount { get; set; }    
}
