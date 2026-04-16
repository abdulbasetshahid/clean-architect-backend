using EShop.Application.Common.Models;
using MediatR;

namespace EShop.Application.Features.Orders.Queries.GetOrderList;

public class GetOrderListQuery : IRequest<PagedResult<OrderListItemVm>>
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
