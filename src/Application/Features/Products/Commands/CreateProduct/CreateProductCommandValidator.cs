using EShop.Application.Features.Products;
using FluentValidation;

namespace EShop.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category is required.");

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Product name is required.")
            .Must(name => name.Trim().Length <= ProductConstraints.NameMaxLength)
            .WithMessage($"Product name must be at most {ProductConstraints.NameMaxLength} characters.");

        RuleFor(x => x.ShortDescription)
            .Must(s => string.IsNullOrEmpty(s) || s!.Length <= ProductConstraints.ShortDescriptionMaxLength)
            .WithMessage($"Short description must be at most {ProductConstraints.ShortDescriptionMaxLength} characters.");

        RuleFor(x => x.Description)
            .Must(s => string.IsNullOrEmpty(s) || s!.Length <= ProductConstraints.DescriptionMaxLength)
            .WithMessage($"Description must be at most {ProductConstraints.DescriptionMaxLength} characters.");

        RuleFor(x => x.ImageUrl)
            .Must(s => string.IsNullOrEmpty(s) || s!.Length <= ProductConstraints.ImageUrlMaxLength)
            .WithMessage($"Image URL must be at most {ProductConstraints.ImageUrlMaxLength} characters.");

        When(x => x.Variants is null || x.Variants.Count == 0, () =>
        {
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Price must be zero or greater.");
        });

        When(x => x.Variants is { Count: > 0 }, () =>
        {
            RuleForEach(x => x.Variants).SetValidator(new ProductVariantDtoValidator());
        });
    }
}
