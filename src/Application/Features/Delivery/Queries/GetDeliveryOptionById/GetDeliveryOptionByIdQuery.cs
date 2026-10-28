using EShop.Application.Features.Delivery.Queries.GetDeliveryOptions;
using MediatR;

namespace EShop.Application.Features.Delivery.Queries.GetDeliveryOptionById;

public class GetDeliveryOptionByIdQuery : IRequest<DeliveryOptionVm>
{
    public Guid Id { get; }

    public GetDeliveryOptionByIdQuery(Guid id) => Id = id;
}
