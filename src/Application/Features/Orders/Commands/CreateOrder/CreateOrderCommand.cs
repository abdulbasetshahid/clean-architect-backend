using EShop.Domain.Enums;
using MediatR;

namespace EShop.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<Guid>
{
    public required string CustomerName { get; set; }

    public required string CustomerPhone { get; set; }

    public required string ShippingAddress { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal ShippingAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;

    public int? ProviderTypeId { get; set; }

    public string? TransactionReference { get; set; }

    public string? ProviderNo { get; set; }

    public required IReadOnlyList<CreateOrderLineItem> Lines { get; set; }
}

public sealed class CreateOrderLineItem
{
    public Guid ProductVariantId { get; set; }

    public int Quantity { get; set; }
}
