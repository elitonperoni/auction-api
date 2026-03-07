using Domain.Entities;

namespace Application.Extensions.Entities;

public static class ProductDetailExtension
{
    public static string GetAddress(this ProductDetail productDetail)
    {
        return $"{productDetail.Country}, {productDetail.State}, {productDetail.City}";
    }
}
