namespace EShop.Application.Contracts;

public interface ICurrentCustomerService
{
    /// <summary>
    /// Firebase UID of the current request, taken only from an identity verified by the Firebase scheme.
    /// </summary>
    string? GetFirebaseUid();
}
