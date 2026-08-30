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

    public async Task<Product?> GetByIdWithDetailsAsync(
        Guid id,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Products
            .Include(p => p.ProductVariants)
            .Where(p => p.Id == id);

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ProductVariant?> GetDetailByIdWithProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductDetails
            .AsNoTracking()
            .Include(d => d.Product)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Product?> GetByIdWithCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.ProductVariants)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> ListWithCategoryAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.ProductVariants)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }
}
