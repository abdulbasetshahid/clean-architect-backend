using EShop.Domain.Enums;
using MediatR;

namespace EShop.Application.Features.Delivery.Commands.CreateDeliveryOption;

public class CreateDeliveryOptionCommand : IRequest<Guid>
{
    public required string Name { get; set; }

    public DeliveryZone Zone { get; set; }

    public decimal Cost { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
