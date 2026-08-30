using EShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Amount).HasPrecision(18, 2);
        builder.Property(e => e.TransactionReference).HasMaxLength(100);
        builder.Property(e => e.ProviderNo).HasMaxLength(50);

        builder.HasIndex(e => e.OrderId);
        builder.HasIndex(e => e.TransactionReference);

        builder.HasOne(e => e.Order)
            .WithMany()
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.PaymentProvider)
            .WithMany()
            .HasForeignKey(e => e.ProviderTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
