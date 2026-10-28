using EShop.Application.Contracts.Persistence;
using MediatR;

namespace EShop.Application.Features.Delivery.Queries.GetDeliveryOptions;

public class GetDeliveryOptionListQueryHandler
    : IRequestHandler<GetDeliveryOptionListQuery, IReadOnlyList<DeliveryOptionVm>>
{
    private readonly IDeliveryOptionRepository _deliveryOptionRepository;

    public GetDeliveryOptionListQueryHandler(IDeliveryOptionRepository deliveryOptionRepository)
    {
        _deliveryOptionRepository = deliveryOptionRepository;
    }

    public async Task<IReadOnlyList<DeliveryOptionVm>> Handle(
        GetDeliveryOptionListQuery request,
        CancellationToken cancellationToken)
    {
        var options = request.IncludeInactive
            ? (await _deliveryOptionRepository.ListAllAsync())
                .OrderBy(o => o.SortOrder)
                .ThenBy(o => o.Name)
                .ToList()
            : await _deliveryOptionRepository.ListActiveAsync(cancellationToken);

        return options.Select(Map).ToList();
    }

    internal static DeliveryOptionVm Map(Domain.Entities.DeliveryOption option) => new()
    {
        Id = option.Id,
        Name = option.Name,
        Zone = option.Zone.ToString(),
        Cost = option.Cost,
        IsActive = option.IsActive,
        SortOrder = option.SortOrder
    };
}
