using EShop.Domain.Entities;

namespace EShop.Application.Contracts.Persistence;

public interface IProductRepository : IAsyncRepository<Product>
{
    Task<Product?> GetByIdWithCategoryAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> ListWithCategoryAsync(CancellationToken cancellationToken = default);
}
