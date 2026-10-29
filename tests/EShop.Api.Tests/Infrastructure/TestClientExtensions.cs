using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace EShop.Api.Tests.Infrastructure;

public static class TestClientExtensions
{
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static HttpClient WithBearer(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static async Task<string> GetAdminTokenAsync(this EShopApiFactory factory)
    {
        var client = factory.CreateClient();
        var userName = $"admin{Guid.NewGuid():N}"[..20];
        var email = $"{userName}@example.com";
        const string password = "Admin#12345";

        var register = await client.PostAsJsonAsync("/api/account/register", new
        {
            firstName = "Test",
            lastName = "Admin",
            email,
            userName,
            password
        });
        register.EnsureSuccessStatusCode();

        var login = await client.PostAsJsonAsync("/api/account/login", new { email, password });
        login.EnsureSuccessStatusCode();

        using var body = JsonDocument.Parse(await login.Content.ReadAsStringAsync());
        return body.RootElement.GetProperty("token").GetString()!;
    }

    public static object CheckoutBody(string? couponCode = null, int quantity = 1) => new
    {
        customerName = "Amina",
        customerPhone = "01700000000",
        customerEmail = "amina@example.com",
        shippingAddress = "Gulshan, Dhaka",
        deliveryOptionId = EShopApiFactory.InsideDhakaId,
        couponCode,
        items = new[] { new { productVariantId = EShopApiFactory.VariantId, quantity } }
    };

    public static async Task<JsonElement> ReadJsonAsync(this HttpResponseMessage response)
    {
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return doc.RootElement.Clone();
    }
}
