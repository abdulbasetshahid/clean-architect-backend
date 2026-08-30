using EShop.Application.Contracts.Persistence;
using EShop.Domain.Entities;
using EShop.Persistence.Repositories.Common;

namespace EShop.Persistence.Repositories;

public class OrderDetailsRepository : BaseRepository<OrderItem>, IOrderDetailsRepository
{
    public OrderDetailsRepository(EShopDbContext dbContext) : base(dbContext)
    {
    }
}
