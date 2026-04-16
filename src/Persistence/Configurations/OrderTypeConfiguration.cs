using EShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Persistence.Configurations;

public class OrderTypeConfiguration : IEntityTypeConfiguration<OrderType>
{
    public void Configure(EntityTypeBuilder<OrderType> builder)
    {
        builder.ToTable("OrderTypes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Type).IsRequired().HasMaxLength(50);

        builder.HasData(new OrderType { Id = 1, Type = "Standard" });
    }
}
