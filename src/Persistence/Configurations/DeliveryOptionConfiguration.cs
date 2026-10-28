using EShop.Domain.Entities;
using EShop.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Persistence.Configurations;

public class DeliveryOptionConfiguration : IEntityTypeConfiguration<DeliveryOption>
{
    public static readonly Guid InsideDhakaId = Guid.Parse("8f3c1a2e-4b6d-4e91-9c07-1a2b3c4d5e6f");
    public static readonly Guid OutsideDhakaId = Guid.Parse("9a4d2b3f-5c7e-4f02-ad18-2b3c4d5e6f70");

    public void Configure(EntityTypeBuilder<DeliveryOption> builder)
    {
        builder.ToTable("DeliveryOptions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Cost).HasPrecision(18, 2);

        builder.HasIndex(e => e.Zone);
        builder.HasIndex(e => e.IsActive);

        builder.HasData(
            new DeliveryOption
            {
                Id = InsideDhakaId,
                Name = "Inside Dhaka",
                Zone = DeliveryZone.InsideDhaka,
                Cost = 80m,
                IsActive = true,
                SortOrder = 1
            },
            new DeliveryOption
            {
                Id = OutsideDhakaId,
                Name = "Outside Dhaka",
                Zone = DeliveryZone.OutsideDhaka,
                Cost = 150m,
                IsActive = true,
                SortOrder = 2
            });
    }
}
