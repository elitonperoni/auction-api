using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IAuctionService
{
    List<string> CreateResponsePhotosUrls(IEnumerable<ProductPhoto> photos);
}
