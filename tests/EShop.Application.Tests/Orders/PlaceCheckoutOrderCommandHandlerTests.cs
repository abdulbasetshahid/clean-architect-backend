using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Application.Features.Orders.Commands.CreateOrder;
using EShop.Application.Features.Orders.Commands.PlaceCheckoutOrder;
using EShop.Domain.Entities;
using EShop.Domain.Enums;
using Moq;

namespace EShop.Application.Tests.Orders;

public class PlaceCheckoutOrderCommandHandlerTests
{
    private readonly Guid _variantId = Guid.NewGuid();
    private readonly Guid _deliveryOptionId = Guid.NewGuid();
    private readonly Mock<ICouponRepository> _coupons = new();
    private Order? _saved;

    [Fact]
    public async Task Handle_GuestCheckout_HasNoCustomerIdentity()
    {
        var response = await CreateHandler().Handle(CreateCommand(), CancellationToken.None);

        Assert.NotNull(_saved);
        Assert.Null(_saved!.CustomerFirebaseUid);
        Assert.Null(_saved.UserId);
        Assert.Equal(_saved.Id, response.OrderId);
        Assert.Equal(_saved.OrderNumber, response.OrderNumber);
    }

    [Fact]
    public async Task Handle_CustomerCheckout_StoresVerifiedFirebaseUid()
    {
        var command = CreateCommand();
        command.FirebaseUid = "firebase-uid-a";

        await CreateHandler().Handle(command, CancellationToken.None);

        Assert.Equal("firebase-uid-a", _saved!.CustomerFirebaseUid);
        Assert.Null(_saved.UserId);
    }

    [Fact]
    public async Task Handle_UsesServerPrices_ZeroTax_AndPendingPayment()
    {
        var response = await CreateHandler().Handle(CreateCommand(quantity: 2), CancellationToken.None);

        var line = Assert.Single(_saved!.OrderDetails);
        Assert.Equal(500m, line.UnitPrice);
        Assert.Equal(1000m, _saved.SubTotal);
        Assert.Equal(0m, _saved.TaxAmount);
        Assert.Equal(0m, _saved.DiscountAmount);
        Assert.Equal(80m, _saved.DeliveryCost);
        Assert.Equal(1080m, _saved.TotalAmount);
        Assert.Equal(1080m, response.TotalAmount);
        Assert.False(_saved.IsPaid);
        Assert.Equal(PaymentStatus.Pending, Assert.Single(_saved.Payments).Status);
        Assert.Equal(OrderStatus.Pending, _saved.Status);
    }

    [Fact]
    public async Task Handle_DuplicateItems_AreMerged()
    {
        var command = CreateCommand();
        command.Items =
        [
            new CreateOrderLineItem { ProductVariantId = _variantId, Quantity = 1 },
            new CreateOrderLineItem { ProductVariantId = _variantId, Quantity = 2 }
        ];

        await CreateHandler().Handle(command, CancellationToken.None);

        Assert.Equal(3, Assert.Single(_saved!.OrderDetails).Quantity);
    }

    [Fact]
    public async Task Handle_PercentageCoupon_DiscountsSubtotal_AndIncrementsUsage()
    {
        var coupon = Coupon("SAVE10", CouponDiscountType.Percentage, 10m);

        var command = CreateCommand(quantity: 2);
        command.CouponCode = " save10 ";
        await CreateHandler().Handle(command, CancellationToken.None);

        Assert.Equal(100m, _saved!.DiscountAmount);
        Assert.Equal(980m, _saved.TotalAmount);
        Assert.Equal(coupon.Id, _saved.CouponId);
        Assert.Equal("SAVE10", _saved.CouponCode);
        Assert.Equal(1, coupon.UsedCount);
    }

    [Fact]
    public async Task Handle_PercentageCoupon_IsCappedByMaxDiscount()
    {
        var coupon = Coupon("HALF", CouponDiscountType.Percentage, 50m);
        coupon.MaxDiscountAmount = 100m;

        var command = CreateCommand(quantity: 2);
        command.CouponCode = "HALF";
        await CreateHandler().Handle(command, CancellationToken.None);

        Assert.Equal(100m, _saved!.DiscountAmount);
    }

    [Fact]
    public async Task Handle_FixedCoupon_IsCappedAtSubtotal()
    {
        Coupon("BIG", CouponDiscountType.FixedAmount, 5000m);

        var command = CreateCommand();
        command.CouponCode = "BIG";
        await CreateHandler().Handle(command, CancellationToken.None);

        Assert.Equal(500m, _saved!.DiscountAmount);
        Assert.Equal(80m, _saved.TotalAmount);
    }

    [Theory]
    [InlineData("expired")]
    [InlineData("inactive")]
    [InlineData("notStarted")]
    [InlineData("limitReached")]
    [InlineData("minAmount")]
    public async Task Handle_CouponNotRedeemable_ThrowsBadRequest_AndDoesNotSave(string scenario)
    {
        var coupon = Coupon("CODE", CouponDiscountType.FixedAmount, 50m);
        switch (scenario)
        {
            case "expired": coupon.ExpiresAt = DateTime.UtcNow.AddMinutes(-1); break;
            case "inactive": coupon.IsActive = false; break;
            case "notStarted": coupon.StartsAt = DateTime.UtcNow.AddDays(1); break;
            case "limitReached": coupon.UsageLimit = 3; coupon.UsedCount = 3; break;
            case "minAmount": coupon.MinOrderAmount = 10_000m; break;
        }

        var command = CreateCommand();
        command.CouponCode = "CODE";

        await Assert.ThrowsAsync<BadRequestException>(() => CreateHandler().Handle(command, CancellationToken.None));
        Assert.Null(_saved);
    }

    [Fact]
    public async Task Handle_UnknownCoupon_ThrowsBadRequest()
    {
        var command = CreateCommand();
        command.CouponCode = "NOPE";

        await Assert.ThrowsAsync<BadRequestException>(() => CreateHandler().Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_OutOfStockVariant_ThrowsBadRequest()
    {
        await Assert.ThrowsAsync<BadRequestException>(() =>
            CreateHandler(inStock: false).Handle(CreateCommand(), CancellationToken.None));
        Assert.Null(_saved);
    }

    private Coupon Coupon(string code, CouponDiscountType type, decimal value)
    {
        var coupon = new Coupon
        {
            Id = Guid.NewGuid(),
            Code = code,
            DiscountType = type,
            Value = value,
            IsActive = true
        };
        _coupons.Setup(r => r.GetByCodeAsync(code, It.IsAny<CancellationToken>())).ReturnsAsync(coupon);
        return coupon;
    }

    private PlaceCheckoutOrderCommandHandler CreateHandler(bool inStock = true)
    {
        var orders = new Mock<IOrderRepository>();
        orders.Setup(r => r.OrderNumberExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        orders.Setup(r => r.AddAsync(It.IsAny<Order>()))
            .Callback<Order>(o => _saved = o)
            .ReturnsAsync((Order o) => o);

        var products = new Mock<IProductRepository>();
        products.Setup(r => r.GetVariantByIdWithProductAsync(_variantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductVariant
            {
                Id = _variantId,
                VariationName = "Default",
                Price = 500m,
                InStock = inStock,
                IsActive = true,
                ProductId = Guid.NewGuid(),
                Product = new Product { Id = Guid.NewGuid(), Name = "Test Product", CategoryId = Guid.NewGuid() }
            });

        var delivery = new Mock<IDeliveryOptionRepository>();
        delivery.Setup(r => r.GetByIdAsync(_deliveryOptionId)).ReturnsAsync(new DeliveryOption
        {
            Id = _deliveryOptionId,
            Name = "Inside Dhaka",
            Zone = DeliveryZone.InsideDhaka,
            Cost = 80m,
            IsActive = true
        });

        return new PlaceCheckoutOrderCommandHandler(orders.Object, products.Object, delivery.Object, _coupons.Object);
    }

    private PlaceCheckoutOrderCommand CreateCommand(int quantity = 1) => new()
    {
        CustomerName = "Amina",
        CustomerPhone = "01700000000",
        CustomerEmail = "amina@example.com",
        ShippingAddress = "Gulshan, Dhaka",
        DeliveryOptionId = _deliveryOptionId,
        Items = [new CreateOrderLineItem { ProductVariantId = _variantId, Quantity = quantity }]
    };
}
