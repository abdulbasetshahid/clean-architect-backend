using EShop.Domain.Common;

namespace EShop.Domain.Entities;

public class Product : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }

    public required string Name { get; set; }
    
    public string? ShortDescription { get; set; }

    public bool IsBestSeller { get; set; }

    public string? ImageUrl { get; set; }

    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = default!;

    public ICollection<ProductVariant> ProductVariants { get; set; } = [];
}