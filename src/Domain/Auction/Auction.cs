using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Users;
using SharedKernel;

namespace Domain.Auction;

public sealed class Auction : Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int BidCount { get; set; }
    public decimal StartingPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public Guid UserId { get; set; }
    public Guid? LastBidderId { get; set; }
    public User? User { get; set; }
    public ICollection<Bid>? Bids { get; set; } = [];
    public ICollection<ProductPhoto>? Photos { get; set; } = [];
}
