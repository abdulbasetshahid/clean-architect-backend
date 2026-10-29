using EShop.Application.Exceptions;
using EShop.Domain.Entities;
using EShop.Domain.Enums;

namespace EShop.Application.Features.Coupons;

internal static class CouponCalculator
{
    /// <summary>
    /// Validates that the coupon can be redeemed now and returns the discount for the given subtotal.
    /// </summary>
    public static decimal CalculateDiscount(Coupon coupon, decimal subTotal, DateTime nowUtc)
    {
        if (!coupon.IsActive)
            throw new BadRequestException("Coupon is not active.");

        if (coupon.StartsAt.HasValue && nowUtc < coupon.StartsAt.Value)
            throw new BadRequestException("Coupon is not yet valid.");

        if (coupon.ExpiresAt.HasValue && nowUtc >= coupon.ExpiresAt.Value)
            throw new BadRequestException("Coupon has expired.");

        if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
            throw new BadRequestException("Coupon usage limit has been reached.");

        if (coupon.MinOrderAmount.HasValue && subTotal < coupon.MinOrderAmount.Value)
            throw new BadRequestException($"Coupon requires a minimum order amount of {coupon.MinOrderAmount.Value:0.##}.");

        var discount = coupon.DiscountType switch
        {
            CouponDiscountType.Percentage => Math.Round(subTotal * coupon.Value / 100m, 2, MidpointRounding.AwayFromZero),
            CouponDiscountType.FixedAmount => coupon.Value,
            _ => 0m
        };

        if (coupon.MaxDiscountAmount.HasValue && discount > coupon.MaxDiscountAmount.Value)
            discount = coupon.MaxDiscountAmount.Value;

        if (discount > subTotal)
            discount = subTotal;

        return discount < 0 ? 0 : discount;
    }
}
