using FluentValidation;

namespace EShop.Application.Features.Delivery.Commands.UpdateDeliveryOption;

public class UpdateDeliveryOptionCommandValidator : AbstractValidator<UpdateDeliveryOptionCommand>
{
    public UpdateDeliveryOptionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(DeliveryConstraints.NameMaxLength);

        RuleFor(x => x.Zone).IsInEnum();

        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);

        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}
