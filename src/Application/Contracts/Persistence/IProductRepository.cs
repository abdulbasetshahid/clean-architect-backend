using EShop.Domain.Entities;

namespace EShop.Application.Contracts.Persistence;

public interface IProductRepository : IAsyncRepository<Product>
{
}
