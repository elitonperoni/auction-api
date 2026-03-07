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
    public int ConditionProductId { get; set; }
    public int ConditionPackagingId { get; set; }
    public int CategoryProductId { get; set; }
    public bool WithoutWarranty { get; set; }
    public string Country { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public List<string>? Photos { get; set; }
}

