using EShop.Application.Contracts;
using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Application.Features.Orders.Commands.CreateOrder;
using EShop.Domain.Entities;
using EShop.Domain.Enums;
using Moq;

namespace EShop.Application.Tests.Orders;

public class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_UsesSelectedDeliveryOptionCost_InOrderTotal()
    {
        var variantId = Guid.NewGuid();
        var insideDhakaId = Guid.NewGuid();
        Order? saved = null;

        var handler = CreateHandler(
            variantId,
            new DeliveryOption
            {
                Id = insideDhakaId,
                Name = "Inside Dhaka",
                Zone = DeliveryZone.InsideDhaka,
                Cost = 80m,
                IsActive = true
            },
            order => saved = order);

        var id = await handler.Handle(CreateCommand(variantId, insideDhakaId, tax: 20m, discount: 10m), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(saved);
        Assert.Equal(80m, saved!.DeliveryCost);
        Assert.Equal(insideDhakaId, saved.DeliveryOptionId);
        Assert.Equal(590m, saved.TotalAmount);
        Assert.Equal(590m, saved.Payments.Single().Amount);
    }

    [Fact]
    public async Task Handle_OutsideDhaka_AppliesHigherDeliveryCost()
    {
        var variantId = Guid.NewGuid();
        var outsideDhakaId = Guid.NewGuid();
        Order? saved = null;

        var handler = CreateHandler(
            variantId,
            new DeliveryOption
            {
                Id = outsideDhakaId,
                Name = "Outside Dhaka",
                Zone = DeliveryZone.OutsideDhaka,
                Cost = 150m,
                IsActive = true
            },
            order => saved = order);

        await handler.Handle(CreateCommand(variantId, outsideDhakaId, tax: 0m, discount: 0m), CancellationToken.None);

        Assert.Equal(150m, saved!.DeliveryCost);
        Assert.Equal(650m, saved.TotalAmount);
    }

    [Fact]
    public async Task Handle_WhenDeliveryOptionIsInactive_ThrowsBadRequest()
    {
        var variantId = Guid.NewGuid();
        var optionId = Guid.NewGuid();

        var handler = CreateHandler(
            variantId,
            new DeliveryOption
            {
                Id = optionId,
                Name = "Inside Dhaka",
                Zone = DeliveryZone.InsideDhaka,
                Cost = 80m,
                IsActive = false
            });

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.Handle(CreateCommand(variantId, optionId), CancellationToken.None));
    }

    private static CreateOrderCommandHandler CreateHandler(
        Guid variantId,
        DeliveryOption option,
        Action<Order>? onSave = null)
    {
        var orders = new Mock<IOrderRepository>();
        orders.Setup(r => r.OrderNumberExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        orders.Setup(r => r.AddAsync(It.IsAny<Order>()))
            .Callback<Order>(o => onSave?.Invoke(o))
            .ReturnsAsync((Order o) => o);

        var products = new Mock<IProductRepository>();
        products.Setup(r => r.GetVariantByIdWithProductAsync(variantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductVariant
            {
                Id = variantId,
                VariationName = "Default",
                Price = 500m,
                InStock = true,
                IsActive = true,
                ProductId = Guid.NewGuid(),
                Product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Product",
                    CategoryId = Guid.NewGuid()
                }
            });

        var delivery = new Mock<IDeliveryOptionRepository>();
        delivery.Setup(r => r.GetByIdAsync(option.Id)).ReturnsAsync(option);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(s => s.GetCurrentUserGuid()).Returns(Guid.NewGuid());

        return new CreateOrderCommandHandler(orders.Object, products.Object, delivery.Object, currentUser.Object);
    }

    private static CreateOrderCommand CreateCommand(
        Guid variantId,
        Guid deliveryOptionId,
        decimal tax = 0m,
        decimal discount = 0m) => new()
    {
        CustomerName = "Amina",
        CustomerPhone = "01700000000",
        ShippingAddress = "Gulshan, Dhaka",
        TaxAmount = tax,
        DiscountAmount = discount,
        DeliveryOptionId = deliveryOptionId,
        Lines = [new CreateOrderLineItem { ProductVariantId = variantId, Quantity = 1 }]
    };
}
