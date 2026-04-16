using EShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.ShortDescription).HasMaxLength(500);
        builder.Property(e => e.Description).HasMaxLength(4000);
        builder.Property(e => e.ImageUrl).HasMaxLength(200);
        builder.Property(e => e.Price).HasPrecision(18, 2);
    }
}
