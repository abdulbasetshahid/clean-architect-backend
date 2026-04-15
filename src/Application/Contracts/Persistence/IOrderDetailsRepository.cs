using EShop.Domain.Entities;

namespace EShop.Application.Contracts.Persistence;

public interface IOrderDetailsRepository : IAsyncRepository<OrderDetails>
{
}
