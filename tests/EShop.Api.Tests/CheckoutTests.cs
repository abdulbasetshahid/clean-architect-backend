using System.Net;
using System.Net.Http.Json;
using EShop.Api.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EShop.Api.Tests;

public class CheckoutTests : IClassFixture<EShopApiFactory>
{
    private readonly EShopApiFactory _factory;

    public CheckoutTests(EShopApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GuestCheckout_WithoutAuthentication_CreatesOrderWithoutCustomer()
    {
        var response = await _factory.CreateClient()
            .PostAsJsonAsync("/api/orders/guest", TestClientExtensions.CheckoutBody(quantity: 2));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Null(response.Headers.Location);

        var body = await response.ReadJsonAsync();
        var orderId = body.GetProperty("orderId").GetGuid();
        Assert.StartsWith("ORD-", body.GetProperty("orderNumber").GetString());

        var order = _factory.WithDb(db => db.Orders.Include(o => o.OrderDetails).Single(o => o.Id == orderId));
        Assert.Null(order.CustomerFirebaseUid);
        Assert.Null(order.UserId);
        Assert.Equal("amina@example.com", order.CustomerEmail);
        Assert.Equal(2 * EShopApiFactory.VariantPrice + EShopApiFactory.InsideDhakaCost, order.TotalAmount);
    }

    [Fact]
    public async Task GuestCheckout_IgnoresFirebaseUidAndTokenSuppliedByClient()
    {
        var client = _factory.CreateClient().WithBearer(FakeFirebaseTokenVerifier.CustomerAToken);

        var response = await client.PostAsJsonAsync("/api/orders/guest", new
        {
            customerName = "Mallory",
            customerPhone = "01700000001",
            shippingAddress = "Dhaka",
            deliveryOptionId = EShopApiFactory.InsideDhakaId,
            firebaseUid = FakeFirebaseTokenVerifier.CustomerBUid,
            customerFirebaseUid = FakeFirebaseTokenVerifier.CustomerBUid,
            items = new[] { new { productVariantId = EShopApiFactory.VariantId, quantity = 1 } }
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var orderId = (await response.ReadJsonAsync()).GetProperty("orderId").GetGuid();
        Assert.Null(_factory.WithDb(db => db.Orders.Single(o => o.Id == orderId).CustomerFirebaseUid));
    }

    [Fact]
    public async Task CustomerCheckout_AssociatesVerifiedUid_AndIgnoresClientTrustedFields()
    {
        var client = _factory.CreateClient().WithBearer(FakeFirebaseTokenVerifier.CustomerAToken);

        var response = await client.PostAsJsonAsync("/api/orders", new
        {
            customerName = "Amina",
            customerPhone = "01700000000",
            shippingAddress = "Gulshan, Dhaka",
            deliveryOptionId = EShopApiFactory.InsideDhakaId,
            firebaseUid = FakeFirebaseTokenVerifier.CustomerBUid,
            customerFirebaseUid = FakeFirebaseTokenVerifier.CustomerBUid,
            userId = Guid.NewGuid(),
            totalAmount = 1m,
            subTotal = 1m,
            taxAmount = 0m,
            discountAmount = 999m,
            isPaid = true,
            items = new[] { new { productVariantId = EShopApiFactory.VariantId, quantity = 1, unitPrice = 1m } }
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        var orderId = (await response.ReadJsonAsync()).GetProperty("orderId").GetGuid();

        var order = _factory.WithDb(db => db.Orders
            .Include(o => o.OrderDetails)
            .Include(o => o.Payments)
            .Single(o => o.Id == orderId));

        Assert.Equal(FakeFirebaseTokenVerifier.CustomerAUid, order.CustomerFirebaseUid);
        Assert.Null(order.UserId);
        Assert.Equal(EShopApiFactory.VariantPrice, order.OrderDetails.Single().UnitPrice);
        Assert.Equal(0m, order.DiscountAmount);
        Assert.Equal(EShopApiFactory.VariantPrice + EShopApiFactory.InsideDhakaCost, order.TotalAmount);
        Assert.False(order.IsPaid);
        Assert.All(order.Payments, p => Assert.Equal(Domain.Enums.PaymentStatus.Pending, p.Status));
    }

    [Fact]
    public async Task Checkout_InvalidPayload_Returns400()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("/api/orders/guest", new
        {
            customerName = "",
            customerPhone = "",
            shippingAddress = "",
            deliveryOptionId = EShopApiFactory.InsideDhakaId,
            items = new[] { new { productVariantId = EShopApiFactory.VariantId, quantity = 0 } }
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Checkout_WithAdminCreatedCoupon_AppliesServerSideDiscount()
    {
        var admin = _factory.CreateClient().WithBearer(await _factory.GetAdminTokenAsync());
        var code = $"SAVE{Guid.NewGuid():N}"[..12];
        var create = await admin.PostAsJsonAsync("/api/coupon", new
        {
            code,
            discountType = 0,
            value = 10m,
            usageLimit = 1,
            isActive = true
        });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var customer = _factory.CreateClient().WithBearer(FakeFirebaseTokenVerifier.CustomerAToken);
        var first = await customer.PostAsJsonAsync("/api/orders", TestClientExtensions.CheckoutBody(code.ToLowerInvariant(), quantity: 2));
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var orderId = (await first.ReadJsonAsync()).GetProperty("orderId").GetGuid();
        var order = _factory.WithDb(db => db.Orders.Single(o => o.Id == orderId));
        Assert.Equal(100m, order.DiscountAmount);
        Assert.Equal(1000m - 100m + EShopApiFactory.InsideDhakaCost, order.TotalAmount);
        Assert.Equal(code.ToUpperInvariant(), order.CouponCode);

        var second = await _factory.CreateClient()
            .PostAsJsonAsync("/api/orders/guest", TestClientExtensions.CheckoutBody(code));
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }
}
