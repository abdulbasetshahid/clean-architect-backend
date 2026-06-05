namespace EShop.Application.Features.Orders.Queries.GetOrderById;

public class OrderLineVm
{
    public Guid ProductDetailId { get; set; }

    public required string ProductName { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }
}
