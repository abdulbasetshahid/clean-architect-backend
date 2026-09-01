using EShop.Application.Contracts.Persistence;
using EShop.Domain.Entities;
using EShop.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace EShop.Persistence.Repositories;

public class DeliveryOptionRepository : BaseRepository<DeliveryOption>, IDeliveryOptionRepository
{
    public DeliveryOptionRepository(EShopDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<DeliveryOption>> ListActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<DeliveryOption>()
            .AsNoTracking()
            .Where(o => o.IsActive)
            .OrderBy(o => o.SortOrder)
            .ThenBy(o => o.Name)
            .ToListAsync(cancellationToken);
    }
}
