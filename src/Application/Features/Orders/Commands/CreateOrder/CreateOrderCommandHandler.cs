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
        var orderNumber = await CreateUniqueOrderNumberAsync(cancellationToken);

        var items = new List<OrderItem>();
        decimal subTotal = 0;

        foreach (var line in request.Lines)
        {
            var variant = await _productRepository.GetVariantByIdWithProductAsync(line.ProductVariantId, cancellationToken);
            if (variant is null)
                throw new NotFoundException(nameof(ProductVariant), line.ProductVariantId);

            var item = OrderItemFactory.Create(orderId, variant, line.Quantity);
            subTotal += item.TotalPrice;
            items.Add(item);
        }

        var totalAmount = subTotal + request.TaxAmount + request.ShippingAmount - request.DiscountAmount;
        if (totalAmount < 0)
            totalAmount = 0;

        var providerTypeId = request.ProviderTypeId ?? PaymentProviderIds.FromMethod(request.PaymentMethod);

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
            DeliveryCost = request.ShippingAmount,
            DiscountAmount = request.DiscountAmount,
            TotalAmount = totalAmount,
            IsPaid = false,
            OrderDetails = items,
            Payments =
            [
                new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    Status = PaymentStatus.Pending,
                    Method = request.PaymentMethod,
                    Amount = totalAmount,
                    TransactionReference = string.IsNullOrWhiteSpace(request.TransactionReference)
                        ? null
                        : request.TransactionReference.Trim(),
                    ProviderNo = string.IsNullOrWhiteSpace(request.ProviderNo)
                        ? null
                        : request.ProviderNo.Trim(),
                    ProviderTypeId = providerTypeId
                }
            ]
        };

        await _orderRepository.AddAsync(order);
        return order.Id;
    }

    private async Task<string> CreateUniqueOrderNumberAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{suffix}";
            if (!await _orderRepository.OrderNumberExistsAsync(orderNumber, cancellationToken))
                return orderNumber;
        }

        throw new BadRequestException("Could not generate a unique order number. Please retry.");
    }
}
