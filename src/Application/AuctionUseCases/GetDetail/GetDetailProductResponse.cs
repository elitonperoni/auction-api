using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AuctionUseCases.GetDetail;

public sealed class GetDetailProductResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal CurrentBid { get; set; }
    public decimal MinBid { get; set; }
    public int BidsCounts { get; set; }
    public string Category { get; set; }
    public bool IsOwner { get; set; }
    public string Seller { get; set; }
    public string Condition { get; set; }
    public string Location { get; set; }
    public List<BidHistoryItem> BidHistory { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string>? Photos { get; set; }
}

public sealed class BidHistoryItem
{
    public string BidderName { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
}
