namespace EShop.Application.Features.Orders.Queries.GetOrderById;

public class OrderPaymentVm
{
    public Guid Id { get; set; }

    public required string Status { get; set; }

    public required string Method { get; set; }

    public decimal Amount { get; set; }

    public string? TransactionReference { get; set; }

    public string? ProviderNo { get; set; }

    public int ProviderTypeId { get; set; }

    public string? ProviderName { get; set; }

    public DateTime? PaidAt { get; set; }
}
