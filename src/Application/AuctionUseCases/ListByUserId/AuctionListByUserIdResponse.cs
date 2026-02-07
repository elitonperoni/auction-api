using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AuctionUseCases.ListByUserId;

public sealed class AuctionListByUserIdResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public decimal CurrentPrice { get; set; }
    public int BidCount { get; set; }
    public string ImageUrl { get; set; }
    public DateTime EndDate { get; set; }
    public string? ActualWinner { get; set; }
    public string Status { get; set; }
}
