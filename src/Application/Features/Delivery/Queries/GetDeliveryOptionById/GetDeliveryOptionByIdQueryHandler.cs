using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Application.Features.Delivery.Queries.GetDeliveryOptions;
using EShop.Domain.Entities;
using MediatR;

namespace EShop.Application.Features.Delivery.Queries.GetDeliveryOptionById;

public class GetDeliveryOptionByIdQueryHandler : IRequestHandler<GetDeliveryOptionByIdQuery, DeliveryOptionVm>
{
    private readonly IDeliveryOptionRepository _deliveryOptionRepository;

    public GetDeliveryOptionByIdQueryHandler(IDeliveryOptionRepository deliveryOptionRepository)
    {
        _deliveryOptionRepository = deliveryOptionRepository;
    }

    public async Task<DeliveryOptionVm> Handle(GetDeliveryOptionByIdQuery request, CancellationToken cancellationToken)
    {
        var option = await _deliveryOptionRepository.GetByIdAsync(request.Id);
        if (option is null)
            throw new NotFoundException(nameof(DeliveryOption), request.Id);

        return GetDeliveryOptionListQueryHandler.Map(option);
    }
}
