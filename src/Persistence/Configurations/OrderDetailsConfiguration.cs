using EShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Persistence.Configurations;

public class OrderDetailsConfiguration : IEntityTypeConfiguration<OrderDetails>
{
    public void Configure(EntityTypeBuilder<OrderDetails> builder)
    {
        builder.ToTable("OrderDetails");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.UnitPrice).HasPrecision(18, 2);
        builder.Property(e => e.LineTotal).HasPrecision(18, 2);

        builder.HasOne<Order>()
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
