using EShop.Domain.Entities;

namespace EShop.Application.Contracts.Persistence;

public interface IDeliveryOptionRepository : IAsyncRepository<DeliveryOption>
{
    Task<IReadOnlyList<DeliveryOption>> ListActiveAsync(CancellationToken cancellationToken = default);
}
