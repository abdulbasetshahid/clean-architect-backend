using FluentValidation;

namespace EShop.Application.Features.Products;

internal sealed class ProductVariantDtoValidator : AbstractValidator<ProductVariantDto>
{
    public ProductVariantDtoValidator()
    {
        RuleFor(x => x.VariationName)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Variation name is required.")
            .Must(name => name.Trim().Length <= ProductConstraints.VariationNameMaxLength)
            .WithMessage($"Variation name must be at most {ProductConstraints.VariationNameMaxLength} characters.");

        RuleFor(x => x.Description)
            .Must(s => string.IsNullOrEmpty(s) || s!.Length <= ProductConstraints.DescriptionMaxLength)
            .WithMessage($"Description must be at most {ProductConstraints.DescriptionMaxLength} characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be zero or greater.");
    }
}
