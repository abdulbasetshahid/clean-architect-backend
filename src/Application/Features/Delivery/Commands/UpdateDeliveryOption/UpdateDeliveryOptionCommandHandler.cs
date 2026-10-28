using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Domain.Entities;
using MediatR;

namespace EShop.Application.Features.Delivery.Commands.UpdateDeliveryOption;

public class UpdateDeliveryOptionCommandHandler : IRequestHandler<UpdateDeliveryOptionCommand, Unit>
{
    private readonly IDeliveryOptionRepository _deliveryOptionRepository;

    public UpdateDeliveryOptionCommandHandler(IDeliveryOptionRepository deliveryOptionRepository)
    {
        _deliveryOptionRepository = deliveryOptionRepository;
    }

    public async Task<Unit> Handle(UpdateDeliveryOptionCommand request, CancellationToken cancellationToken)
    {
        var option = await _deliveryOptionRepository.GetByIdAsync(request.Id);
        if (option is null)
            throw new NotFoundException(nameof(DeliveryOption), request.Id);

        option.Name = request.Name.Trim();
        option.Zone = request.Zone;
        option.Cost = request.Cost;
        option.SortOrder = request.SortOrder;
        option.IsActive = request.IsActive;

        await _deliveryOptionRepository.UpdateAsync(option);
        return Unit.Value;
    }
}
