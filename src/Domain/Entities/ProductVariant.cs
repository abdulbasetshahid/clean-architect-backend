using EShop.Domain.Common;

namespace EShop.Domain.Entities;

public class ProductVariant : AuditableEntity, IEntity<Guid>
    {
        public Guid Id { get; set; }
        public required string VariationName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; } = 0;
        public bool InStock { get; set; } = true;
        public Guid ProductId { get; set; }

        public bool IsActive { get; set; }
        public Product Product { get; set; } = null!;
    }
