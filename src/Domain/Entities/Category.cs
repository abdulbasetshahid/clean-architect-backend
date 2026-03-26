using Domain.Common;

namespace Domain.Entities;

public class Category : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Product> Products { get; set; } = default!;
}