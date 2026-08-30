using EShop.Application.Features.Orders;
using FluentValidation;

namespace EShop.Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

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

        RuleFor(x => x.Lines)
            .Must(lines => lines is null || lines.Count > 0)
            .WithMessage("When provided, lines must contain at least one item.");

        When(x => x.Lines is not null, () =>
        {
            RuleForEach(x => x.Lines!).ChildRules(line =>
            {
                line.RuleFor(l => l.ProductVariantId).NotEmpty();
                line.RuleFor(l => l.Quantity).GreaterThan(0);
            });
        });
    }
}
