namespace EShop.Application.Features.Delivery.Queries.GetDeliveryOptions;

public class DeliveryOptionVm
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Zone { get; set; }

    public decimal Cost { get; set; }

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }
}
