namespace EShop.Application.Contracts;

public interface ICurrentUserService
{
    string? GetCurrentUserId();

    Guid? GetCurrentUserGuid();
}
