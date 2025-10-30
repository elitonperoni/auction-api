using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;

namespace Application.AuctionBid.Create;

public sealed class CreateAuctionBidCommand : ICommand<Guid>
{
    public Guid UserId { get; set; }
    public decimal Value { get; set; }
    public DateTime? DueDate { get; set; }
}
