namespace EShop.Application.Features.Orders.Queries.GetOrderById;

public class OrderDetailVm
{
    public Guid Id { get; set; }

    public required string OrderCode { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime? ShippedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public required string Status { get; set; }

    public required string CustomerName { get; set; }

    public required string CustomerPhone { get; set; }

    public required string ShippingAddress { get; set; }

    public decimal SubTotal { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal ShippingAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public bool IsPaid { get; set; }

    public IReadOnlyList<OrderLineVm> Lines { get; set; } = [];

    public IReadOnlyList<OrderPaymentVm> Payments { get; set; } = [];
}
