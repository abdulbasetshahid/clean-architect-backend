using MediatR;

namespace EShop.Application.Features.Delivery.Queries.GetDeliveryOptions;

public class GetDeliveryOptionListQuery : IRequest<IReadOnlyList<DeliveryOptionVm>>
{
    public bool IncludeInactive { get; set; }
}
