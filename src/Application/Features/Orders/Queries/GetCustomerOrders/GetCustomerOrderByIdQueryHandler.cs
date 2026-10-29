using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Application.Features.Orders.Queries.GetOrderById;
using EShop.Domain.Entities;
using MediatR;

namespace EShop.Application.Features.Orders.Queries.GetCustomerOrders;

public class GetCustomerOrderByIdQueryHandler : IRequestHandler<GetCustomerOrderByIdQuery, OrderDetailVm>
{
    private readonly IOrderRepository _orderRepository;

    public GetCustomerOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDetailVm> Handle(GetCustomerOrderByIdQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FirebaseUid))
            throw new UnauthorizedException("Customer authentication is required.");

        // Orders that do not belong to the caller are reported as not found so their existence is not revealed.
        var order = await _orderRepository.GetByIdWithDetailsForCustomerAsync(
            request.Id,
            request.FirebaseUid,
            cancellationToken);

        if (order is null)
            throw new NotFoundException(nameof(Order), request.Id);

        return GetOrderByIdQueryHandler.Map(order);
    }
}
