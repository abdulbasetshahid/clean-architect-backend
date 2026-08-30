using EShop.Domain.Entities;

namespace EShop.Application.Contracts.Persistence;

public interface IProductRepository : IAsyncRepository<Product>
{
    Task<Product?> GetByIdWithDetailsAsync(Guid id, bool asNoTracking = true, CancellationToken cancellationToken = default);

    Task<ProductVariant?> GetDetailByIdWithProductAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Product?> GetByIdWithCategoryAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> ListWithCategoryAsync(CancellationToken cancellationToken = default);
}
