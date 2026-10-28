using EShop.Application.Exceptions;
using EShop.Domain.Entities;

namespace EShop.Application.Features.Orders;

internal static class OrderItemFactory
{
    public static OrderItem Create(Guid orderId, ProductVariant variant, int quantity)
    {
        var productName = variant.Product.Name;

        if (!variant.IsActive)
            throw new BadRequestException($"Product variant '{variant.VariationName}' is not available.");

        if (!variant.InStock)
            throw new BadRequestException($"Product '{productName}' is not in stock.");

        var unitPrice = variant.Price;
        var totalPrice = unitPrice * quantity;

        return new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = variant.ProductId,
            ProductName = productName,
            ProductVariantId = variant.Id,
            Quantity = quantity,
            UnitPrice = unitPrice,
            TotalPrice = totalPrice
        };
    }
}
