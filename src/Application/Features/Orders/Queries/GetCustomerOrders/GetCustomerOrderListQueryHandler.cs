using EShop.Application.Common.Models;
using EShop.Application.Contracts.Persistence;
using EShop.Application.Features.Orders.Queries.GetOrderList;
using MediatR;

namespace EShop.Application.Features.Orders.Queries.GetCustomerOrders;

public class GetCustomerOrderListQueryHandler
    : IRequestHandler<GetCustomerOrderListQuery, PagedResult<OrderListItemVm>>
{
    private readonly IOrderRepository _orderRepository;

    public GetCustomerOrderListQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedResult<OrderListItemVm>> Handle(
        GetCustomerOrderListQuery request,
        CancellationToken cancellationToken)
    {
        var (orders, totalCount) = await _orderRepository.GetPagedForCustomerAsync(
            request.FirebaseUid,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return new PagedResult<OrderListItemVm>
        {
            Items = orders.Select(GetOrderListQueryHandler.Map).ToList(),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
