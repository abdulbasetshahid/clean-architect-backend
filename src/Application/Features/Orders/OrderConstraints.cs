namespace EShop.Application.Features.Orders;

public static class OrderConstraints
{
    public const int CustomerNameMaxLength = 200;
    public const int CustomerPhoneMaxLength = 50;
    public const int ShippingAddressMaxLength = 500;
    public const int MaxPageSize = 100;
    public const int CustomerEmailMaxLength = 256;
    public const int MaxCheckoutItems = 50;
    public const int MaxLineQuantity = 100;
}
