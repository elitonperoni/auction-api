using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;

namespace Application.AuctionUseCases.SendBid;

public sealed class SendBidCommand : ICommand<int>
{
    public Guid UserId { get; set; }
    public Guid AuctionId { get; set; }    
    public decimal BidPrice { get; set; }    
}
