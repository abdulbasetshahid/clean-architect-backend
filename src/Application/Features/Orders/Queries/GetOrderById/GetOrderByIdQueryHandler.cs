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
            ShippingAddress = order.ShippingAddress,
            SubTotal = order.SubTotal,
            TaxAmount = order.TaxAmount,
            ShippingAmount = order.DeliveryCost,
            DiscountAmount = order.DiscountAmount,
            TotalAmount = order.TotalAmount,
            IsPaid = order.IsPaid,
            Lines = order.OrderDetails.Select(d => new OrderLineVm
            {
                ProductDetailId = d.ProductVariantId,
                ProductName = d.ProductVariant.Product.Name,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                LineTotal = d.TotalPrice
            }).ToList()
        };
    }
}
