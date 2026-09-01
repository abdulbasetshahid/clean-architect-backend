namespace EShop.Application.Features.Orders;

internal static class OrderTotals
{
    public static decimal Calculate(decimal subTotal, decimal taxAmount, decimal deliveryCost, decimal discountAmount)
    {
        var total = subTotal + taxAmount + deliveryCost - discountAmount;
        return total < 0 ? 0 : total;
    }
}
