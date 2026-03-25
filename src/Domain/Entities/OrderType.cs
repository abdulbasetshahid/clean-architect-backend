using Domain.Common;

namespace Domain.Entities;

public class OrderType : IEntity<int>
{
    public int Id { get; set; }

    public required string Type { get; set; }
}
