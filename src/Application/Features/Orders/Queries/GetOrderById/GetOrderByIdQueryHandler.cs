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
            ShippedAt = order.ShippedAt,
            DeliveredAt = order.DeliveredAt,
            CancelledAt = order.CancelledAt,
            Status = order.Status.ToString(),
            CustomerName = order.CustomerName,
            CustomerPhone = order.CustomerPhone,
            ShippingAddress = order.ShippingAddress,
            SubTotal = order.SubTotal,
            TaxAmount = order.TaxAmount,
            ShippingAmount = order.DeliveryCost,
            DeliveryOptionId = order.DeliveryOptionId,
            DeliveryOptionName = order.DeliveryOption?.Name,
            DeliveryZone = order.DeliveryOption?.Zone.ToString(),
            DiscountAmount = order.DiscountAmount,
            TotalAmount = order.TotalAmount,
            IsPaid = order.IsPaid,
            Lines = order.OrderDetails.Select(d => new OrderLineVm
            {
                ProductId = d.ProductId,
                ProductVariantId = d.ProductVariantId,
                ProductName = d.ProductName ?? d.ProductVariant.Product.Name,
                VariationName = d.ProductVariant.VariationName,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                TotalPrice = d.TotalPrice
            }).ToList(),
            Payments = order.Payments.Select(p => new OrderPaymentVm
            {
                Id = p.Id,
                Status = p.Status.ToString(),
                Method = p.Method.ToString(),
                Amount = p.Amount,
                TransactionReference = p.TransactionReference,
                ProviderNo = p.ProviderNo,
                ProviderTypeId = p.ProviderTypeId,
                ProviderName = p.PaymentProvider?.ProviderName,
                PaidAt = p.PaidAt
            }).ToList()
        };
    }
}
