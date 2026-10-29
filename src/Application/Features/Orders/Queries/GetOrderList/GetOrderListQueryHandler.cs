using EShop.Application.Common.Models;
using EShop.Application.Contracts.Persistence;
using EShop.Domain.Entities;
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

        var items = orders.Select(Map).ToList();

        return new PagedResult<OrderListItemVm>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    internal static OrderListItemVm Map(Order order) => new()
    {
        Id = order.Id,
        OrderCode = order.OrderNumber,
        TotalProductQuantity = order.OrderDetails.Sum(d => d.Quantity),
        Status = order.Status.ToString(),
        TotalPrice = order.TotalAmount,
        IsPaid = order.IsPaid,
        UserName = order.CustomerName,
        UserPhone = order.CustomerPhone
    };
}
