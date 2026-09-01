using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Domain.Entities;

namespace EShop.Application.Features.Orders;

internal static class DeliveryCostResolver
{
    public static async Task<DeliveryOption> GetActiveAsync(
        IDeliveryOptionRepository repository,
        Guid deliveryOptionId,
        CancellationToken cancellationToken)
    {
        _ = cancellationToken;

        var option = await repository.GetByIdAsync(deliveryOptionId);
        if (option is null)
            throw new NotFoundException(nameof(DeliveryOption), deliveryOptionId);

        if (!option.IsActive)
            throw new BadRequestException($"Delivery option '{option.Name}' is not available.");

        return option;
    }
}
