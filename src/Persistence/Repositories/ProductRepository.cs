using EShop.Application.Contracts.Persistence;
using EShop.Domain.Entities;
using EShop.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace EShop.Persistence.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(EShopDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Product?> GetByIdWithCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> ListWithCategoryAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }
}
