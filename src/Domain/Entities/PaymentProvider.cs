using EShop.Domain.Common;

namespace EShop.Domain.Entities;

public class PaymentProvider : IEntity<int>
{
    public int Id { get; set; }

    public required string ProviderName { get; set; }
}
