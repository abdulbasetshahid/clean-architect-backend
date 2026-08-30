using EShop.Application.Features.Orders;
using FluentValidation;

namespace EShop.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .MaximumLength(OrderConstraints.CustomerNameMaxLength);

        RuleFor(x => x.CustomerPhone)
            .NotEmpty()
            .MaximumLength(OrderConstraints.CustomerPhoneMaxLength);

        RuleFor(x => x.ShippingAddress)
            .NotEmpty()
            .MaximumLength(OrderConstraints.ShippingAddressMaxLength);

        RuleFor(x => x.TaxAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ShippingAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.DiscountAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.PaymentMethod)
            .IsInEnum();

        RuleFor(x => x.TransactionReference)
            .MaximumLength(100);

        RuleFor(x => x.ProviderNo)
            .MaximumLength(50);

        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("At least one order line is required.");

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.ProductVariantId).NotEmpty();
            line.RuleFor(l => l.Quantity).GreaterThan(0);
        });
    }
}
