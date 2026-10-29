using System.Net;
using System.Net.Http.Json;
using EShop.Api.Tests.Infrastructure;

namespace EShop.Api.Tests;

public class CustomerOrderHistoryTests : IClassFixture<EShopApiFactory>
{
    private readonly EShopApiFactory _factory;

    public CustomerOrderHistoryTests(EShopApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Customers_SeeOnlyTheirOwnOrders()
    {
        var customerA = _factory.CreateClient().WithBearer(FakeFirebaseTokenVerifier.CustomerAToken);
        var customerB = _factory.CreateClient().WithBearer(FakeFirebaseTokenVerifier.CustomerBToken);

        var orderA = await PlaceAsync(customerA, "/api/orders");
        var orderB = await PlaceAsync(customerB, "/api/orders");
        var guestOrder = await PlaceAsync(_factory.CreateClient(), "/api/orders/guest");

        var listA = await customerA.GetAsync("/api/customer/orders?pageNumber=1&pageSize=100");
        Assert.Equal(HttpStatusCode.OK, listA.StatusCode);
        var idsA = (await listA.ReadJsonAsync()).GetProperty("items").EnumerateArray()
            .Select(i => i.GetProperty("id").GetGuid())
            .ToList();

        Assert.Contains(orderA, idsA);
        Assert.DoesNotContain(orderB, idsA);
        Assert.DoesNotContain(guestOrder, idsA);

        Assert.Equal(HttpStatusCode.OK, (await customerA.GetAsync($"/api/customer/orders/{orderA}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await customerA.GetAsync($"/api/customer/orders/{orderB}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await customerA.GetAsync($"/api/customer/orders/{guestOrder}")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await customerB.GetAsync($"/api/customer/orders/{orderB}")).StatusCode);
    }

    [Fact]
    public async Task CustomerHistory_IgnoresCustomerIdentityInQueryString()
    {
        var customerB = _factory.CreateClient().WithBearer(FakeFirebaseTokenVerifier.CustomerBToken);
        var orderB = await PlaceAsync(customerB, "/api/orders");

        var customerA = _factory.CreateClient().WithBearer(FakeFirebaseTokenVerifier.CustomerAToken);
        var response = await customerA.GetAsync(
            $"/api/customer/orders?firebaseUid={FakeFirebaseTokenVerifier.CustomerBUid}&customerId={FakeFirebaseTokenVerifier.CustomerBUid}&pageSize=100");

        var ids = (await response.ReadJsonAsync()).GetProperty("items").EnumerateArray()
            .Select(i => i.GetProperty("id").GetGuid());
        Assert.DoesNotContain(orderB, ids);
    }

    [Fact]
    public async Task GuestCannotAccessCustomerHistory()
    {
        var guest = _factory.CreateClient();
        var guestOrder = await PlaceAsync(guest, "/api/orders/guest");

        Assert.Equal(HttpStatusCode.Unauthorized, (await guest.GetAsync("/api/customer/orders")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await guest.GetAsync($"/api/customer/orders/{guestOrder}")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await guest.GetAsync($"/api/order/{guestOrder}")).StatusCode);
    }

    private static async Task<Guid> PlaceAsync(HttpClient client, string url)
    {
        var response = await client.PostAsJsonAsync(url, TestClientExtensions.CheckoutBody());
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.ReadJsonAsync()).GetProperty("orderId").GetGuid();
    }
}
