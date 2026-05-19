using Application.Common.Enums;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Infrastructure.Services;

public class AuctionService(IS3Service s3Service) : IAuctionService
{
    public List<string> CreateResponsePhotosUrls(IEnumerable<ProductPhoto> photos)
    {
        List<string> photoUrls = new();
        foreach (ProductPhoto photo in photos)
        {
            Uri photoUrl = s3Service.BuildPublicUri($"{AWSS3Folder.AuctionProductPhotos.GetDescription()}/{photo.AuctionId.ToString()}/{photo.Name}");
            photoUrls.Add(photoUrl.ToString());
        }
        return photoUrls;
    }
}
