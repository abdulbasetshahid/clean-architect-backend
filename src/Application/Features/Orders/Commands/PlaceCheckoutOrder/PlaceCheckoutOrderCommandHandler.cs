using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Application.Features.Coupons;
using EShop.Domain.Entities;
using EShop.Domain.Enums;
using MediatR;

namespace EShop.Application.Features.Orders.Commands.PlaceCheckoutOrder;

public class PlaceCheckoutOrderCommandHandler : IRequestHandler<PlaceCheckoutOrderCommand, CheckoutOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IDeliveryOptionRepository _deliveryOptionRepository;
    private readonly ICouponRepository _couponRepository;

    public PlaceCheckoutOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IDeliveryOptionRepository deliveryOptionRepository,
        ICouponRepository couponRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _deliveryOptionRepository = deliveryOptionRepository;
        _couponRepository = couponRepository;
    }

    public async Task<CheckoutOrderResponse> Handle(PlaceCheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        var orderId = Guid.NewGuid();
        var orderNumber = await OrderNumberGenerator.CreateUniqueAsync(_orderRepository, cancellationToken);
        var deliveryOption = await DeliveryCostResolver.GetActiveAsync(
            _deliveryOptionRepository,
            request.DeliveryOptionId,
            cancellationToken);

        var groupedItems = request.Items
            .GroupBy(item => item.ProductVariantId)
            .Select(group => (VariantId: group.Key, Quantity: group.Sum(item => item.Quantity)))
            .ToList();

        var orderItems = new List<OrderItem>();
        decimal subTotal = 0;

        foreach (var (variantId, quantity) in groupedItems)
        {
            if (quantity > OrderConstraints.MaxLineQuantity)
                throw new BadRequestException($"Quantity per product cannot exceed {OrderConstraints.MaxLineQuantity}.");

            var variant = await _productRepository.GetVariantByIdWithProductAsync(variantId, cancellationToken);
            if (variant is null)
                throw new NotFoundException(nameof(ProductVariant), variantId);

            var orderItem = OrderItemFactory.Create(orderId, variant, quantity);
            subTotal += orderItem.TotalPrice;
            orderItems.Add(orderItem);
        }

        Coupon? coupon = null;
        decimal discountAmount = 0;
        if (!string.IsNullOrWhiteSpace(request.CouponCode))
        {
            var code = CouponConstraints.NormalizeCode(request.CouponCode);
            coupon = await _couponRepository.GetByCodeAsync(code, cancellationToken)
                ?? throw new BadRequestException("Coupon code is not valid.");

            discountAmount = CouponCalculator.CalculateDiscount(coupon, subTotal, DateTime.UtcNow);
            coupon.UsedCount++;
        }

        const decimal taxAmount = 0m;
        var totalAmount = OrderTotals.Calculate(subTotal, taxAmount, deliveryOption.Cost, discountAmount);
        var providerTypeId = request.ProviderTypeId ?? PaymentProviderIds.FromMethod(request.PaymentMethod);

        var order = new Order
        {
            Id = orderId,
            UserId = null,
            CustomerFirebaseUid = string.IsNullOrWhiteSpace(request.FirebaseUid) ? null : request.FirebaseUid,
            CustomerName = request.CustomerName.Trim(),
            CustomerPhone = request.CustomerPhone.Trim(),
            CustomerEmail = string.IsNullOrWhiteSpace(request.CustomerEmail) ? null : request.CustomerEmail.Trim(),
            OrderNumber = orderNumber,
            ShippingAddress = request.ShippingAddress.Trim(),
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            DeliveryOptionId = deliveryOption.Id,
            CouponId = coupon?.Id,
            CouponCode = coupon?.Code,
            SubTotal = subTotal,
            TaxAmount = taxAmount,
            DeliveryCost = deliveryOption.Cost,
            DiscountAmount = discountAmount,
            TotalAmount = totalAmount,
            IsPaid = false,
            OrderDetails = orderItems,
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

        // The tracked coupon's UsedCount increment is saved in the same SaveChanges as the order.
        await _orderRepository.AddAsync(order);

        return new CheckoutOrderResponse
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            TotalAmount = order.TotalAmount
        };
    }
}
