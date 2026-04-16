using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Domain.Entities;
using MediatR;

namespace EShop.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailVm>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDetailVm> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(request.Id, asNoTracking: true, cancellationToken);
        if (order is null)
            throw new NotFoundException(nameof(Order), request.Id);

        return new OrderDetailVm
        {
            Id = order.Id,
            OrderCode = order.OrderNumber,
            OrderDate = order.OrderDate,
            Status = order.Status.ToString(),
            CustomerName = order.CustomerName,
            CustomerPhone = order.CustomerPhone,
            SubTotal = order.SubTotal,
            TaxAmount = order.TaxAmount,
            ShippingAmount = order.ShippingAmount,
            DiscountAmount = order.DiscountAmount,
            TotalAmount = order.TotalAmount,
            IsPaid = order.IsPaid,
            OrderTypeId = order.OrderTypeId,
            OrderType = order.OrderType.Type,
            Lines = order.OrderDetails.Select(d => new OrderLineVm
            {
                ProductId = d.ProductId,
                ProductName = d.Product.Name,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                LineTotal = d.LineTotal
            }).ToList()
        };
    }
}
