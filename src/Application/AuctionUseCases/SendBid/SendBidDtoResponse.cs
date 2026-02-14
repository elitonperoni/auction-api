using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AuctionUseCases.SendBid;

public sealed class SendBidDtoResponse
{
    public Guid AuctionId { get; set; }
    public int TotalBids { get; set; }
    public Guid LastBidderId { get; set; }
    public string LastBidderNamer { get; set; }
    public Guid AuctionOwnerId { get; set; }
    public string MessageToOwner { get; set; }
    public DateTime Date { get; set; }    
    public decimal Amount { get; set; }    
}
