using EShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Persistence.Configurations;

public class PaymentProviderConfiguration : IEntityTypeConfiguration<PaymentProvider>
{
    public void Configure(EntityTypeBuilder<PaymentProvider> builder)
    {
        builder.ToTable("PaymentProviders");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.ProviderName).IsRequired().HasMaxLength(50);

        builder.HasIndex(e => e.ProviderName).IsUnique();

        builder.HasData(
            new PaymentProvider { Id = 1, ProviderName = "CashOnDelivery" },
            new PaymentProvider { Id = 2, ProviderName = "bKash" },
            new PaymentProvider { Id = 3, ProviderName = "Nagad" },
            new PaymentProvider { Id = 4, ProviderName = "CreditCard" },
            new PaymentProvider { Id = 5, ProviderName = "DebitCard" },
            new PaymentProvider { Id = 6, ProviderName = "BankTransfer" });
    }
}
