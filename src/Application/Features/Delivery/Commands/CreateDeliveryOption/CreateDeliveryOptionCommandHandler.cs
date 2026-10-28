using EShop.Application.Contracts.Persistence;
using EShop.Domain.Entities;
using MediatR;

namespace EShop.Application.Features.Delivery.Commands.CreateDeliveryOption;

public class CreateDeliveryOptionCommandHandler : IRequestHandler<CreateDeliveryOptionCommand, Guid>
{
    private readonly IDeliveryOptionRepository _deliveryOptionRepository;

    public CreateDeliveryOptionCommandHandler(IDeliveryOptionRepository deliveryOptionRepository)
    {
        _deliveryOptionRepository = deliveryOptionRepository;
    }

    public async Task<Guid> Handle(CreateDeliveryOptionCommand request, CancellationToken cancellationToken)
    {
        var option = new DeliveryOption
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Zone = request.Zone,
            Cost = request.Cost,
            SortOrder = request.SortOrder,
            IsActive = request.IsActive
        };

        await _deliveryOptionRepository.AddAsync(option);
        return option.Id;
    }
}
