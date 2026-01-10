using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AuctionUseCases.SendBid;

public sealed class SendBidDtoResponse
{
    public int TotalBids { get; set; }
    public DateTime Date { get; set; }    
    public decimal Amount { get; set; }    
}
