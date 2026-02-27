using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SharedKernel;
using SharedKernel.Enum;

namespace Domain.Entities;

public sealed class ProductDetail : Entity
{
    public Guid Id { get; set; } = Guid.NewGuid(); 
    public string Description { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal StartingPrice { get; set; }
    public int ConditionProductId { get; set; } 
    public int ConditionPackagingId { get; set; } 
    public int CategoryProductId { get; set; } 
    public bool WithoutWarranty { get; set; }
    public CategoryProduct? CategoryProduct { get; set; }
    public ConditionProduct? ConditionProduct { get; set; }
    public ConditionPackaging? ConditionPackaging { get; set; }
}
