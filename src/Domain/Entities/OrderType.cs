using EShop.Domain.Common;

namespace EShop.Domain.Entities;

public class OrderType : IEntity<int>
{
    public int Id { get; set; }

    public required string Type { get; set; }
}
