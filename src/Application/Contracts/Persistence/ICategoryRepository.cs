using EShop.Domain.Entities;

namespace EShop.Application.Contracts.Persistence
{
    public interface ICategoryRepository: IAsyncRepository<Category>
    {
        Task<bool> HasProductsInCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    }
}
