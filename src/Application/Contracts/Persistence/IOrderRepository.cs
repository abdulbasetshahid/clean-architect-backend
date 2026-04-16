using EShop.Domain.Entities;

namespace EShop.Application.Contracts.Persistence;

public interface IOrderRepository : IAsyncRepository<Order>
{
    Task<Order?> GetByIdWithDetailsAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken);

    Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedWithDetailsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task CommitAsync(CancellationToken cancellationToken);

    Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken);

    void ClearOrderDetails(Order trackedOrder);
}
