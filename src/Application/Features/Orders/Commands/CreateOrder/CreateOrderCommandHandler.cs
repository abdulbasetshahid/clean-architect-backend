using EShop.Application.Contracts;
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
    private readonly ICurrentUserService _currentUserService;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICurrentUserService currentUserService)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderId = Guid.NewGuid();
        var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}";

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
            UserId = _currentUserService.GetCurrentUserGuid(),
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
            DeliveryDate = null,         
        };

        await _orderRepository.AddAsync(order);
        return order.Id;
    }
}
