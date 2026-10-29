using System.Text.Json.Serialization;
using EShop.Application.Features.Orders.Commands.CreateOrder;
using EShop.Domain.Enums;
using MediatR;

namespace EShop.Application.Features.Orders.Commands.PlaceCheckoutOrder;

/// <summary>
/// Storefront checkout used by both guest and Firebase-authenticated customers.
/// Prices, tax, discount and totals are always calculated on the server.
/// </summary>
public class PlaceCheckoutOrderCommand : IRequest<CheckoutOrderResponse>
{
    public required string CustomerName { get; set; }

    public required string CustomerPhone { get; set; }

    public string? CustomerEmail { get; set; }

    public required string ShippingAddress { get; set; }

    public Guid DeliveryOptionId { get; set; }

    public string? CouponCode { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;

    public int? ProviderTypeId { get; set; }

    public string? TransactionReference { get; set; }

    public string? ProviderNo { get; set; }

    public required IReadOnlyList<CreateOrderLineItem> Items { get; set; }

    /// <summary>Set only by the API from a verified Firebase identity; null for guest checkout.</summary>
    [JsonIgnore]
    public string? FirebaseUid { get; set; }
}

public sealed class CheckoutOrderResponse
{
    public Guid OrderId { get; set; }

    public required string OrderNumber { get; set; }

    public decimal TotalAmount { get; set; }
}
