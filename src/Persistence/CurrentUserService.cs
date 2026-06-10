using System.Security.Claims;
using EShop.Application.Contracts;
using Microsoft.AspNetCore.Http;

namespace EShop.Persistence;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        return user?.FindFirst("uid")?.Value
            ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public Guid? GetCurrentUserGuid()
    {
        var userId = GetCurrentUserId();

        return Guid.TryParse(userId, out var currentUserId)
            ? currentUserId
            : null;
    }
}
