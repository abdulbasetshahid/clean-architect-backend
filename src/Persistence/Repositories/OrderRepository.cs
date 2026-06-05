using EShop.Application.Contracts.Persistence;
using EShop.Domain.Entities;
using EShop.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace EShop.Persistence.Repositories;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(EShopDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Order?> GetByIdWithDetailsAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<Order>()
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.ProductDetail)
                    .ThenInclude(d => d.Product)
            .Where(o => o.Id == id);

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedWithDetailsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<Order>()
            .AsNoTracking()
            .Include(o => o.OrderDetails)
            .OrderByDescending(o => o.OrderDate);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task CommitAsync(CancellationToken cancellationToken) =>
        _dbContext.SaveChangesAsync(cancellationToken);

    public async Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Order>()
            .AsNoTracking()
            .AnyAsync(o => o.OrderNumber == orderNumber, cancellationToken);
    }

    public void ClearOrderDetails(Order trackedOrder)
    {
        var set = _dbContext.Set<OrderDetails>();
        foreach (var detail in trackedOrder.OrderDetails.ToList())
            set.Remove(detail);

        trackedOrder.OrderDetails.Clear();
    }
}
