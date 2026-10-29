namespace EShop.Application.Features.Coupons;

public static class CouponConstraints
{
    public const int CodeMaxLength = 50;
    public const int DescriptionMaxLength = 500;
    public const string CodePattern = "^[A-Za-z0-9_-]+$";

    public static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();
}
