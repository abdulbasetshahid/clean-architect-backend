namespace EShop.Application.Features.Orders.Queries.GetOrderById;

public class OrderLineVm
{
    public Guid ProductId { get; set; }

    public Guid ProductVariantId { get; set; }

    public required string ProductName { get; set; }

    public string? VariationName { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }
}
