using SharedKernel;

namespace Domain.Entities;

public sealed class ProductPhoto : Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AuctionId { get; set; } 
    public string Name { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public Auction? Auction { get; set; }
}
