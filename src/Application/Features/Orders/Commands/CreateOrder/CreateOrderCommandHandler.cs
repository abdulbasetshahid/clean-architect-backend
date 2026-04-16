using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Domain.Entities;
using EShop.Domain.Enums;
using MediatR;

namespace EShop.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderTypeRepository _orderTypeRepository;
    private readonly IProductRepository _productRepository;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IOrderTypeRepository orderTypeRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _orderTypeRepository = orderTypeRepository;
        _productRepository = productRepository;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _ = await _orderTypeRepository.GetByIdAsync(request.OrderTypeId)
            ?? throw new NotFoundException(nameof(OrderType), request.OrderTypeId);

        var orderId = Guid.NewGuid();
        var orderNumber = await GenerateUniqueOrderNumberAsync(cancellationToken);

        var details = new List<OrderDetails>();
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

            details.Add(new OrderDetails
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = product.Id,
                Quantity = line.Quantity,
                UnitPrice = unitPrice,
                LineTotal = lineTotal
            });
        }

        var totalAmount = subTotal + request.TaxAmount + request.ShippingAmount - request.DiscountAmount;
        if (totalAmount < 0)
            totalAmount = 0;

        var order = new Order
        {
            Id = orderId,
            UserId = request.UserId,
            CustomerName = request.CustomerName.Trim(),
            CustomerPhone = request.CustomerPhone.Trim(),
            OrderNumber = orderNumber,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            SubTotal = subTotal,
            TaxAmount = request.TaxAmount,
            ShippingAmount = request.ShippingAmount,
            DiscountAmount = request.DiscountAmount,
            TotalAmount = totalAmount,
            IsPaid = false,
            OrderTypeId = request.OrderTypeId,
            OrderDetails = details,
            CreatedBy = "api",
            LastModifiedBy = "api"
        };

        await _orderRepository.AddAsync(order);
        return order.Id;
    }

    private async Task<string> GenerateUniqueOrderNumberAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var candidate = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}";

            if (!await _orderRepository.OrderNumberExistsAsync(candidate, cancellationToken))
                return candidate;
        }

        return $"ORD-{Guid.NewGuid():N}";
    }
}
