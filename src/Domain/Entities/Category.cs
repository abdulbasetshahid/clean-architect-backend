using Domain.Common;

namespace Domain.Entities;

public class Category: AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}