using EShop.Application.Features.Orders.Commands.CreateOrder;
using MediatR;

namespace EShop.Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public required string CustomerName { get; set; }

    public required string CustomerPhone { get; set; }

    public required string ShippingAddress { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal ShippingAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public bool IsPaid { get; set; }

    public Guid? UserId { get; set; }

    public IReadOnlyList<CreateOrderLineItem>? Lines { get; set; }
}

