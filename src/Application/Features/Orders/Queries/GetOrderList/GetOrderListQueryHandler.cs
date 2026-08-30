using EShop.Application.Common.Models;
using EShop.Application.Contracts.Persistence;
using MediatR;

namespace EShop.Application.Features.Orders.Queries.GetOrderList;

public class GetOrderListQueryHandler : IRequestHandler<GetOrderListQuery, PagedResult<OrderListItemVm>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderListQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedResult<OrderListItemVm>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
    {
        var (orders, totalCount) = await _orderRepository.GetPagedWithDetailsAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var items = orders.Select(o => new OrderListItemVm
        {
            Id = o.Id,
            OrderCode = o.OrderNumber,
            TotalProductQuantity = o.OrderDetails.Sum(d => d.Quantity),
            Status = o.Status.ToString(),
            TotalPrice = o.TotalAmount,
            IsPaid = o.IsPaid,
            UserName = o.CustomerName,
            UserPhone = o.CustomerPhone
        }).ToList();

        return new PagedResult<OrderListItemVm>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
