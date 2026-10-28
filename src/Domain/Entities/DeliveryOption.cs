using EShop.Domain.Common;
using EShop.Domain.Enums;

namespace EShop.Domain.Entities;

public class DeliveryOption : IEntity<Guid>
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public DeliveryZone Zone { get; set; }

    public decimal Cost { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    public ICollection<Order> Orders { get; set; } = [];
}
