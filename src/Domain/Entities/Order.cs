using EShop.Domain.Common;
using EShop.Domain.Enums;

namespace EShop.Domain.Entities;

public class Order : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }

    public required string CustomerName { get; set; }

    public required string CustomerPhone { get; set; }

    public required string OrderNumber { get; set; }

    public required string ShippingAddress {get; set; }

    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal DeliveryCost { get; set; }
    public decimal TotalAmount { get; set; }

    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public bool IsPaid { get; set; }

    public ICollection<OrderItem> OrderDetails { get; set; } = default!;
}
