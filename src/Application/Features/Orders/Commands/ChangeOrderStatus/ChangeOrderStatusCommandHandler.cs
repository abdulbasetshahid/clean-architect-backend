using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Domain.Entities;
using MediatR;

namespace EShop.Application.Features.Orders.Commands.ChangeOrderStatus;

public class ChangeOrderStatusCommandHandler : IRequestHandler<ChangeOrderStatusCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;

    public ChangeOrderStatusCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Unit> Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        if (order is null)
            throw new NotFoundException(nameof(Order), request.OrderId);

        order.Status = request.Status;
        await _orderRepository.UpdateAsync(order);
        return Unit.Value;
    }
}
