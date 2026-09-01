using FluentValidation;

namespace EShop.Application.Features.Delivery.Commands.CreateDeliveryOption;

public class CreateDeliveryOptionCommandValidator : AbstractValidator<CreateDeliveryOptionCommand>
{
    public CreateDeliveryOptionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(DeliveryConstraints.NameMaxLength);

        RuleFor(x => x.Zone).IsInEnum();

        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);

        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}
