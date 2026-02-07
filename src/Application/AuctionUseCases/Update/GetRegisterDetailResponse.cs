using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.AuctionUseCases.GetDetail;

namespace Application.AuctionUseCases.Update;

public class GetRegisterDetailResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal InitialValue { get; set; }
    public DateTime EndDate { get; set; }
    public List<string>? Photos { get; set; }
}

