using EShop.Application.Features.Orders.Queries.GetOrderById;
using MediatR;

namespace EShop.Application.Features.Orders.Queries.GetCustomerOrders;

public class GetCustomerOrderByIdQuery : IRequest<OrderDetailVm>
{
    public GetCustomerOrderByIdQuery(Guid id, string firebaseUid)
    {
        Id = id;
        FirebaseUid = firebaseUid;
    }

    public Guid Id { get; }

    public string FirebaseUid { get; }
}
