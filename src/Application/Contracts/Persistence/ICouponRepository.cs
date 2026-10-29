using EShop.Domain.Entities;

namespace EShop.Application.Contracts.Persistence;

public interface ICouponRepository : IAsyncRepository<Coupon>
{
    /// <summary>Returns a tracked coupon by its normalized (upper-case) code.</summary>
    Task<Coupon?> GetByCodeAsync(string normalizedCode, CancellationToken cancellationToken);

    Task<bool> CodeExistsAsync(string normalizedCode, Guid? excludeId, CancellationToken cancellationToken);
}
