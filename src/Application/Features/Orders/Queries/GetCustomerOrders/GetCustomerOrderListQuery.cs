using EShop.Application.Common.Models;
using EShop.Application.Features.Orders.Queries.GetOrderList;
using MediatR;

namespace EShop.Application.Features.Orders.Queries.GetCustomerOrders;

public class GetCustomerOrderListQuery : IRequest<PagedResult<OrderListItemVm>>
{
    public GetCustomerOrderListQuery(string firebaseUid, int pageNumber, int pageSize)
    {
        FirebaseUid = firebaseUid;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public string FirebaseUid { get; }

    public int PageNumber { get; }

    public int PageSize { get; }
}
