using EShop.Domain.Common;
using EShop.Domain.Enums;

namespace EShop.Domain.Entities;

public class Coupon : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }

    public required string Code { get; set; }

    public string? Description { get; set; }

    public CouponDiscountType DiscountType { get; set; }

    public decimal Value { get; set; }

    public decimal? MaxDiscountAmount { get; set; }

    public decimal? MinOrderAmount { get; set; }

    public DateTime? StartsAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public int? UsageLimit { get; set; }

    public int UsedCount { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Order> Orders { get; set; } = [];
}
