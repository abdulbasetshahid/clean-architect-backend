using EShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Persistence.Configurations;

public class OrderTypeConfiguration : IEntityTypeConfiguration<PaymentProvider>
{
    public void Configure(EntityTypeBuilder<PaymentProvider> builder)
    {
        builder.ToTable("OrderTypes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.ProviderName).IsRequired().HasMaxLength(50);

        builder.HasData(new PaymentProvider { Id = 1, ProviderName = "Standard" });
    }
}
