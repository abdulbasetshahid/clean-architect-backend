using EShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.OrderNumber).IsUnique();

        builder.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
        builder.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.CustomerPhone).IsRequired().HasMaxLength(50);
        builder.Property(e => e.ShippingAddress).IsRequired().HasMaxLength(500);
        builder.Property(e => e.SubTotal).HasPrecision(18, 2);
        builder.Property(e => e.TaxAmount).HasPrecision(18, 2);
        builder.Property(e => e.DeliveryCost).HasPrecision(18, 2);
        builder.Property(e => e.DiscountAmount).HasPrecision(18, 2);
        builder.Property(e => e.TotalAmount).HasPrecision(18, 2);

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.DeliveryOptionId);

        builder.HasOne(e => e.DeliveryOption)
            .WithMany(o => o.Orders)
            .HasForeignKey(e => e.DeliveryOptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
