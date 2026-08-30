using EShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ProductName).HasMaxLength(200);
        builder.Property(e => e.UnitPrice).HasPrecision(18, 2);
        builder.Property(e => e.TotalPrice).HasPrecision(18, 2);

        builder.HasIndex(e => e.OrderId);
        builder.HasIndex(e => e.ProductVariantId);

        builder.HasOne(e => e.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.ProductVariant)
            .WithMany()
            .HasForeignKey(e => e.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
