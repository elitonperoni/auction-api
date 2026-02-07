using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Auction;

namespace Domain.Interfaces;

public interface IAuctionService
{
    List<string> CreateResponsePhotosUrls(IEnumerable<ProductPhoto> photos);
}
