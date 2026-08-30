using EShop.Domain.Common;

namespace EShop.Domain.Entities;

public class OrderItem : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; } = null;
    public Guid ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public Order Order { get; set; } = null!;
    public ProductVariant ProductVariant { get; set; } = null!;
}
