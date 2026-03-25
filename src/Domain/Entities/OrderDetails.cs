using Domain.Common;

namespace Domain.Entities;

public class OrderDetails : IEntity<Guid>
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid OrderId { get; set; }


}
