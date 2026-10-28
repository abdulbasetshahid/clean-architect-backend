using EShop.Domain.Enums;

namespace EShop.Application.Features.Orders;

internal static class PaymentProviderIds
{
    public static int FromMethod(PaymentMethod method) => method switch
    {
        PaymentMethod.CashOnDelivery => 1,
        PaymentMethod.BKash => 2,
        PaymentMethod.Nagad => 3,
        PaymentMethod.CreditCard => 4,
        PaymentMethod.DebitCard => 5,
        PaymentMethod.BankTransfer => 6,
        _ => 1
    };
}
