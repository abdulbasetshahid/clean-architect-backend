using EShop.Application.Contracts.Persistence;
using EShop.Domain.Entities;
using EShop.Persistence.Repositories.Common;

namespace EShop.Persistence.Repositories;

public class OrderTypeRepository : BaseRepository<OrderType>, IOrderTypeRepository
{
    public OrderTypeRepository(EShopDbContext dbContext) : base(dbContext)
    {
    }
}
