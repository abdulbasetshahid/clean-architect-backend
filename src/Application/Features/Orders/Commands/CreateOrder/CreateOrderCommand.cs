using MediatR;

namespace EShop.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<Guid>
{
    public Guid? UserId { get; set; }

    public required string CustomerName { get; set; }

    public required string CustomerPhone { get; set; }

    public required string ShippingAddress { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal ShippingAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public required IReadOnlyList<CreateOrderLineItem> Lines { get; set; }
}

public sealed class CreateOrderLineItem
{
    public Guid ProductDetailId { get; set; }

    public int Quantity { get; set; }
}
