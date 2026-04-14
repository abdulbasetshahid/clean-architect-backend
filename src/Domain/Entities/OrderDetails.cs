using EShop.Domain.Common;

namespace EShop.Domain.Entities;

public class OrderDetails : IEntity<Guid>
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid OrderId { get; set; }


}
