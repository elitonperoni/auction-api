using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

    [Column(TypeName = "timestamp with time zone")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime EndDate { get; set; }
    public int BidCount { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal StartingPrice { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal CurrentPrice { get; set; }
    public Guid UserId { get; set; }
    public Guid? LastBidderId { get; set; }
    public User? User { get; set; }

    [ForeignKey(nameof(LastBidderId))]
    public User? LastBidder { get; set; }
    public ICollection<Bid>? Bids { get; set; } = [];
    public ICollection<ProductPhoto>? Photos { get; set; } = [];
}
