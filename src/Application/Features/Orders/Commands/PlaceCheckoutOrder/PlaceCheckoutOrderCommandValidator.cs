using EShop.Application.Features.Coupons;
using FluentValidation;

namespace EShop.Application.Features.Orders.Commands.PlaceCheckoutOrder;

public class PlaceCheckoutOrderCommandValidator : AbstractValidator<PlaceCheckoutOrderCommand>
{
    public PlaceCheckoutOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .MaximumLength(OrderConstraints.CustomerNameMaxLength);

        RuleFor(x => x.CustomerPhone)
            .NotEmpty()
            .MaximumLength(OrderConstraints.CustomerPhoneMaxLength);

        RuleFor(x => x.CustomerEmail)
            .EmailAddress()
            .MaximumLength(OrderConstraints.CustomerEmailMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.CustomerEmail));

        RuleFor(x => x.ShippingAddress)
            .NotEmpty()
            .MaximumLength(OrderConstraints.ShippingAddressMaxLength);

        RuleFor(x => x.DeliveryOptionId)
            .NotEmpty();

        RuleFor(x => x.CouponCode)
            .MaximumLength(CouponConstraints.CodeMaxLength)
            .Matches(CouponConstraints.CodePattern)
            .When(x => !string.IsNullOrWhiteSpace(x.CouponCode));

        RuleFor(x => x.PaymentMethod)
            .IsInEnum();

        RuleFor(x => x.TransactionReference)
            .MaximumLength(100);

        RuleFor(x => x.ProviderNo)
            .MaximumLength(50);

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one order item is required.");

        RuleFor(x => x.Items)
            .Must(items => items is null || items.Count <= OrderConstraints.MaxCheckoutItems)
            .WithMessage($"An order can contain at most {OrderConstraints.MaxCheckoutItems} items.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductVariantId).NotEmpty();
            item.RuleFor(i => i.Quantity).InclusiveBetween(1, OrderConstraints.MaxLineQuantity);
        });
    }
}
