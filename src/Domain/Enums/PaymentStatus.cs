namespace EShop.Domain.Enums;

public enum PaymentStatus
{
    Pending,
    Authorized,
    Paid,
    Failed,
    Refunded,
    PartiallyRefunded
}
