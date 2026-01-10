using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Auction;

public sealed class ProductPhoto : Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AuctionId { get; set; } 
    public string Name { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public Auction? Auction { get; set; }
}
