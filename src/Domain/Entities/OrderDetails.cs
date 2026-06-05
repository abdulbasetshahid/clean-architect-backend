using EShop.Domain.Common;

namespace EShop.Domain.Entities;

public class OrderDetails : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductDetailId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public Order Order { get; set; } = null!;
    public ProductDetail ProductDetail { get; set; } = null!;
}
