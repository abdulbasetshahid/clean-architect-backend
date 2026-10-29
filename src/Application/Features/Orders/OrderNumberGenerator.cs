using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;

namespace EShop.Application.Features.Orders;

internal static class OrderNumberGenerator
{
    public static async Task<string> CreateUniqueAsync(IOrderRepository orderRepository, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{suffix}";
            if (!await orderRepository.OrderNumberExistsAsync(orderNumber, cancellationToken))
                return orderNumber;
        }

        throw new BadRequestException("Could not generate a unique order number. Please retry.");
    }
}
