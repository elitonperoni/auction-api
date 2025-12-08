using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Users;
using SharedKernel;

namespace Domain.Auction;

public sealed class Bid : Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AuctionId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime BidDate { get; set; }
    public Auction? Auction { get; set; }
    public User? User { get; set; }
}
