using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Domain.Entities;
using EShop.Domain.Enums;
using MediatR;

namespace EShop.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderId = Guid.NewGuid();
        var orderNumber = await GenerateUniqueOrderNumberAsync(cancellationToken);

        var details = new List<OrderDetails>();
        decimal subTotal = 0;

        foreach (var line in request.Lines)
        {
            var detail = await _productRepository.GetDetailByIdWithProductAsync(line.ProductDetailId, cancellationToken);
            if (detail is null)
                throw new NotFoundException(nameof(ProductDetail), line.ProductDetailId);

            var productName = detail.Product.Name;

            if (!detail.InStock)
                throw new BadRequestException($"Product '{productName}' is not in stock.");

            var unitPrice = detail.Price;
            var lineTotal = unitPrice * line.Quantity;
            subTotal += lineTotal;

            details.Add(new OrderDetails
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductDetailId = detail.Id,
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
            ShippingAddress = request.ShippingAddress.Trim(),
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            SubTotal = subTotal,
            TaxAmount = request.TaxAmount,
            DeliveryFee = request.ShippingAmount,
            DiscountAmount = request.DiscountAmount,
            TotalAmount = totalAmount,
            IsPaid = false,
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
