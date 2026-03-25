using Domain.Common;

namespace Domain.Entities;

public class Order: AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }

    public int OrderTypeId { get; set; }
    public double OrderTotal { get; set; }
    public DateTime OrderPlaced { get; set; }
    public bool OrderPaid { get; set; }

    public required OrderType OrderType { get; set; }
}
