using EShop.Domain.Common;

public class Product : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
}