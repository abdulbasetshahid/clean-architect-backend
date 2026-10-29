using EShop.Domain.Entities;

namespace EShop.Application.Features.Coupons.Queries;

public class CouponVm
{
    public Guid Id { get; set; }

    public required string Code { get; set; }

    public string? Description { get; set; }

    public required string DiscountType { get; set; }

    public decimal Value { get; set; }

    public decimal? MaxDiscountAmount { get; set; }

    public decimal? MinOrderAmount { get; set; }

    public DateTime? StartsAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public int? UsageLimit { get; set; }

    public int UsedCount { get; set; }

    public bool IsActive { get; set; }

    internal static CouponVm Map(Coupon coupon) => new()
    {
        Id = coupon.Id,
        Code = coupon.Code,
        Description = coupon.Description,
        DiscountType = coupon.DiscountType.ToString(),
        Value = coupon.Value,
        MaxDiscountAmount = coupon.MaxDiscountAmount,
        MinOrderAmount = coupon.MinOrderAmount,
        StartsAt = coupon.StartsAt,
        ExpiresAt = coupon.ExpiresAt,
        UsageLimit = coupon.UsageLimit,
        UsedCount = coupon.UsedCount,
        IsActive = coupon.IsActive
    };
}
