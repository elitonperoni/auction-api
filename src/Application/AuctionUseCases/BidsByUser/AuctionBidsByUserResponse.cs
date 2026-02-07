using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AuctionUseCases.BidsByUser;

public sealed class AuctionBidsByUserResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal UserLastBidAmount { get; set; }
    public int BidCount { get; set; }
    public int UserBidsCount { get; set; }
    public string ImageUrl { get; set; }
    public string ActualLeader { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsUserActualLeader { get; set; }    
    public bool IsUserWinner { get; set; }    
}
