using EShop.Domain.Enums;
using MediatR;

namespace EShop.Application.Features.Delivery.Commands.UpdateDeliveryOption;

public class UpdateDeliveryOptionCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public DeliveryZone Zone { get; set; }

    public decimal Cost { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }
}
