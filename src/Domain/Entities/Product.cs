using EShop.Domain.Common;

namespace EShop.Domain.Entities;

public class Product : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }

    public required string Name { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool InStock { get; set; } = true;
    public bool IsBestSeller { get; set; }

    public string? ImageUrl { get; set; }

    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = default!;

    public ICollection<ProductDetail> ProductDetails { get; set; } = [];
}