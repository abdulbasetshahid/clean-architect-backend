using EShop.Domain.Enums;

namespace EShop.Api.Models;

public class ChangeOrderStatusRequest
{
    public OrderStatus Status { get; set; }
}
