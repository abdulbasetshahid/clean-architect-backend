using MediatR;

namespace EShop.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQuery : IRequest<OrderDetailVm>
{
    public Guid Id { get; }

    public GetOrderByIdQuery(Guid id) => Id = id;
}
