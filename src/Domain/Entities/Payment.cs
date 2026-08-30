using EShop.Domain.Enums;

namespace EShop.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }

    public PaymentStatus Status { get; set; }
    public PaymentMethod Method { get; set; }

    public decimal Amount { get; set; }
    public string? TransactionReference { get; set; }
    public string? ProviderNo { get; set; }

    public int ProviderTypeId { get; set; }

    public DateTime? PaidAt { get; set; }

    public Order Order { get; set; } = null!;

    public PaymentProvider? PaymentProvider { get; set; } = null;
}
