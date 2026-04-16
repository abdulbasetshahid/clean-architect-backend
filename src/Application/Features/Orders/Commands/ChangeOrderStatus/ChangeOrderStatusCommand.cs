using EShop.Domain.Enums;
using MediatR;

namespace EShop.Application.Features.Orders.Commands.ChangeOrderStatus;

public class ChangeOrderStatusCommand : IRequest<Unit>
{
    public Guid OrderId { get; set; }

    public OrderStatus Status { get; set; }
}
