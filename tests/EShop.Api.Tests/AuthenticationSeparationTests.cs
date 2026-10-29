using System.Net;
using System.Net.Http.Json;
using EShop.Api.Tests.Infrastructure;

namespace EShop.Api.Tests;

public class AuthenticationSeparationTests : IClassFixture<EShopApiFactory>
{
    private readonly EShopApiFactory _factory;

    public AuthenticationSeparationTests(EShopApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AdminLogin_IssuesJwt_ThatAuthorizesAdminEndpoints()
    {
        var token = await _factory.GetAdminTokenAsync();
        var client = _factory.CreateClient().WithBearer(token);

        var orders = await client.GetAsync("/api/order");
        var coupons = await client.GetAsync("/api/coupon");

        Assert.Equal(HttpStatusCode.OK, orders.StatusCode);
        Assert.Equal(HttpStatusCode.OK, coupons.StatusCode);
    }

    [Theory]
    [InlineData("/api/order")]
    [InlineData("/api/order/00000000-0000-0000-0000-000000000001")]
    [InlineData("/api/coupon")]
    public async Task AdminEndpoints_Anonymous_Return401(string url)
    {
        var response = await _factory.CreateClient().GetAsync(url);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/order")]
    [InlineData("/api/coupon")]
    [InlineData("/api/order/00000000-0000-0000-0000-000000000001")]
    public async Task AdminEndpoints_RejectFirebaseCustomerToken(string url)
    {
        var client = _factory.CreateClient().WithBearer(FakeFirebaseTokenVerifier.CustomerAToken);

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AdminCreateOrder_RejectsFirebaseCustomerToken()
    {
        var client = _factory.CreateClient().WithBearer(FakeFirebaseTokenVerifier.CustomerAToken);

        var response = await client.PostAsJsonAsync("/api/order", TestClientExtensions.CheckoutBody());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/customer/orders")]
    [InlineData("/api/customer/orders/00000000-0000-0000-0000-000000000001")]
    public async Task CustomerEndpoints_RejectAdminJwt(string url)
    {
        var token = await _factory.GetAdminTokenAsync();
        var client = _factory.CreateClient().WithBearer(token);

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CustomerCheckout_RejectsAdminJwt()
    {
        var token = await _factory.GetAdminTokenAsync();
        var client = _factory.CreateClient().WithBearer(token);

        var response = await client.PostAsJsonAsync("/api/orders", TestClientExtensions.CheckoutBody());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("not-a-jwt")]
    [InlineData("eyJhbGciOiJSUzI1NiJ9.eyJleHAiOjF9.c2ln")]
    [InlineData("expired-firebase-token")]
    [InlineData("wrong-project-firebase-token")]
    public async Task CustomerEndpoints_RejectInvalidFirebaseTokens(string token)
    {
        var client = _factory.CreateClient().WithBearer(token);

        var list = await client.GetAsync("/api/customer/orders");
        var checkout = await client.PostAsJsonAsync("/api/orders", TestClientExtensions.CheckoutBody());

        Assert.Equal(HttpStatusCode.Unauthorized, list.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, checkout.StatusCode);
    }

    [Fact]
    public async Task CustomerEndpoints_MissingCredentials_Return401()
    {
        var client = _factory.CreateClient();

        var list = await client.GetAsync("/api/customer/orders");
        var checkout = await client.PostAsJsonAsync("/api/orders", TestClientExtensions.CheckoutBody());

        Assert.Equal(HttpStatusCode.Unauthorized, list.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, checkout.StatusCode);
        Assert.Equal("application/problem+json", list.Content.Headers.ContentType?.MediaType);
    }
}
