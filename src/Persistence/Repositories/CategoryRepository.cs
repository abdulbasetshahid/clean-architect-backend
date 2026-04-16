using EShop.Application.Contracts.Persistence;
using EShop.Domain.Entities;
using EShop.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace EShop.Persistence.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(EShopDbContext dbContext) : base(dbContext)
    {
    }

    public Task<bool> HasProductsInCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Products
            .AnyAsync(p => p.CategoryId == categoryId, cancellationToken);
    }
}
