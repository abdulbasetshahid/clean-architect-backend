using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Domain.Entities;
using MediatR;

namespace EShop.Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderTypeRepository _orderTypeRepository;
    private readonly IProductRepository _productRepository;

    public UpdateOrderCommandHandler(
        IOrderRepository orderRepository,
        IOrderTypeRepository orderTypeRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _orderTypeRepository = orderTypeRepository;
        _productRepository = productRepository;
    }

    public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(request.Id, asNoTracking: false, cancellationToken);
        if (order is null)
            throw new NotFoundException(nameof(Order), request.Id);

        _ = await _orderTypeRepository.GetByIdAsync(request.OrderTypeId)
            ?? throw new NotFoundException(nameof(OrderType), request.OrderTypeId);

        order.CustomerName = request.CustomerName.Trim();
        order.CustomerPhone = request.CustomerPhone.Trim();
        order.OrderTypeId = request.OrderTypeId;
        order.TaxAmount = request.TaxAmount;
        order.ShippingAmount = request.ShippingAmount;
        order.DiscountAmount = request.DiscountAmount;
        order.IsPaid = request.IsPaid;
        order.UserId = request.UserId;

        if (request.Lines is not null)
        {
            _orderRepository.ClearOrderDetails(order);

            decimal subTotal = 0;
            foreach (var line in request.Lines)
            {
                var product = await _productRepository.GetByIdAsync(line.ProductId);
                if (product is null)
                    throw new NotFoundException(nameof(Product), line.ProductId);

                if (!product.InStock)
                    throw new BadRequestException($"Product '{product.Name}' is not in stock.");

                var unitPrice = product.Price;
                var lineTotal = unitPrice * line.Quantity;
                subTotal += lineTotal;

                order.OrderDetails.Add(new OrderDetails
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = line.Quantity,
                    UnitPrice = unitPrice,
                    LineTotal = lineTotal
                });
            }

            order.SubTotal = subTotal;
        }

        order.TotalAmount = order.SubTotal + order.TaxAmount + order.ShippingAmount - order.DiscountAmount;
        if (order.TotalAmount < 0)
            order.TotalAmount = 0;

        await _orderRepository.CommitAsync(cancellationToken);
        return Unit.Value;
    }
}
