using EShop.Domain.Enums;

namespace EShop.Application.Features.Coupons;

public abstract class CouponCommandBase
{
    public required string Code { get; set; }

    public string? Description { get; set; }

    public CouponDiscountType DiscountType { get; set; }

    public decimal Value { get; set; }

    public decimal? MaxDiscountAmount { get; set; }

    public decimal? MinOrderAmount { get; set; }

    public DateTime? StartsAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public int? UsageLimit { get; set; }

    public bool IsActive { get; set; } = true;
}
