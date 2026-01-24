using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AuctionUseCases.List;

public sealed class AuctionListResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public decimal CurrentPrice { get; set; }
    public int BidCount { get; set; }
    public string ImageUrl { get; set; }
    public DateTime EndDate { get; set; }
}
