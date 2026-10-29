using EShop.Application.Contracts.Persistence;
using EShop.Domain.Entities;
using EShop.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace EShop.Persistence.Repositories;

public class CouponRepository : BaseRepository<Coupon>, ICouponRepository
{
    public CouponRepository(EShopDbContext dbContext) : base(dbContext)
    {
    }

    public Task<Coupon?> GetByCodeAsync(string normalizedCode, CancellationToken cancellationToken)
    {
        return _dbContext.Set<Coupon>()
            .FirstOrDefaultAsync(c => c.Code == normalizedCode, cancellationToken);
    }

    public Task<bool> CodeExistsAsync(string normalizedCode, Guid? excludeId, CancellationToken cancellationToken)
    {
        return _dbContext.Set<Coupon>()
            .AsNoTracking()
            .AnyAsync(c => c.Code == normalizedCode && (excludeId == null || c.Id != excludeId), cancellationToken);
    }
}
