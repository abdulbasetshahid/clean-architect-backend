using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Domain.Entities;
using MediatR;

namespace EShop.Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public UpdateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(request.Id, asNoTracking: false, cancellationToken);
        if (order is null)
            throw new NotFoundException(nameof(Order), request.Id);

        order.CustomerName = request.CustomerName.Trim();
        order.CustomerPhone = request.CustomerPhone.Trim();
        order.ShippingAddress = request.ShippingAddress.Trim();
        order.TaxAmount = request.TaxAmount;
        order.DeliveryCost = request.ShippingAmount;
        order.DiscountAmount = request.DiscountAmount;
        order.IsPaid = request.IsPaid;
        order.UserId = request.UserId;

        if (request.Lines is not null)
        {
            _orderRepository.ClearOrderDetails(order);

            decimal subTotal = 0;
            foreach (var line in request.Lines)
            {
                var variant = await _productRepository.GetVariantByIdWithProductAsync(line.ProductVariantId, cancellationToken);
                if (variant is null)
                    throw new NotFoundException(nameof(ProductVariant), line.ProductVariantId);

                var item = OrderItemFactory.Create(order.Id, variant, line.Quantity);
                subTotal += item.TotalPrice;
                order.OrderDetails.Add(item);
            }

            order.SubTotal = subTotal;
        }

        order.TotalAmount = order.SubTotal + order.TaxAmount + order.DeliveryCost - order.DiscountAmount;
        if (order.TotalAmount < 0)
            order.TotalAmount = 0;

        await _orderRepository.CommitAsync(cancellationToken);
        return Unit.Value;
    }
}
