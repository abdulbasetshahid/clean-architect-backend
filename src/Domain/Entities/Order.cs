using EShop.Domain.Common;
using EShop.Domain.Enums;

namespace EShop.Domain.Entities;

public class Order : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }

    public required string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public bool IsPaid { get; set; }

    public int OrderTypeId { get; set; }
    public required OrderType OrderType { get; set; }

    public ICollection<OrderDetails> OrderDetails { get; set; } = default!;
}
