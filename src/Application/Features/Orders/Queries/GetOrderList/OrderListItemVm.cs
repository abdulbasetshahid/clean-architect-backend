namespace EShop.Application.Features.Orders.Queries.GetOrderList;

public class OrderListItemVm
{
    public Guid Id { get; set; }

    public required string OrderCode { get; set; }

    public int TotalProductQuantity { get; set; }

    public required string Status { get; set; }

    public decimal TotalPrice { get; set; }

    public bool IsPaid { get; set; }

    public required string UserName { get; set; }

    public required string UserPhone { get; set; }
}
