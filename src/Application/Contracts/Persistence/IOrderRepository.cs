using EShop.Domain.Entities;

namespace EShop.Application.Contracts.Persistence
{
    public interface IOrderRepository : IAsyncRepository<Order>
    {
    }
}
