using EShop.Domain.Common;
using EShop.Domain.Enums;

namespace EShop.Domain.Entities;

public class Order : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }

    public required string CustomerName { get; set; }

    public required string CustomerPhone { get; set; }

    public required string OrderNumber { get; set; }

    public required string ShippingAddress {get; set; }

    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal TotalAmount { get; set; }

    public bool IsPaid { get; set; }

    public ICollection<OrderDetails> OrderDetails { get; set; } = default!;
}
