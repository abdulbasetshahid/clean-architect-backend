using EShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Persistence.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupons");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(e => e.Code).IsUnique();

        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.Value).HasPrecision(18, 2);
        builder.Property(e => e.MaxDiscountAmount).HasPrecision(18, 2);
        builder.Property(e => e.MinOrderAmount).HasPrecision(18, 2);

        // Prevents two concurrent checkouts from both redeeming the last allowed use.
        builder.Property(e => e.UsedCount).IsConcurrencyToken();

        builder.HasIndex(e => e.IsActive);
    }
}
