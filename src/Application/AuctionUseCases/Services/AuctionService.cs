using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Enums;
using Application.Extensions;
using Application.Interfaces;
using Domain.Auction;
using Domain.Interfaces;

namespace Application.AuctionUseCases.Services;

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
