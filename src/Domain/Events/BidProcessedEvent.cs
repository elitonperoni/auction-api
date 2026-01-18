using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events;

public record BidProcessedEvent(
    Guid AuctionId, 
    decimal TotalAmount, 
    int TotalBids, 
    Guid LastBidderId, 
    string LastBidderNamer, 
    DateTime ProcessedAt);
