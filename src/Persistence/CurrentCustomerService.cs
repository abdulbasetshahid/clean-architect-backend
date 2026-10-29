using EShop.Application.Contracts;
using EShop.Application.Models.Authentication;
using Microsoft.AspNetCore.Http;

namespace EShop.Persistence;

public class CurrentCustomerService : ICurrentCustomerService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentCustomerService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetFirebaseUid()
    {
        var identity = _httpContextAccessor.HttpContext?.User.Identities.FirstOrDefault(i =>
            i.IsAuthenticated &&
            i.AuthenticationType == CustomerAuthenticationDefaults.AuthenticationScheme);

        var uid = identity?.FindFirst(CustomerAuthenticationDefaults.FirebaseUidClaimType)?.Value;
        return string.IsNullOrWhiteSpace(uid) ? null : uid;
    }
}
