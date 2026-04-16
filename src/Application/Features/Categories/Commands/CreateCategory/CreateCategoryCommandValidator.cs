using EShop.Application.Features.Categories;
using FluentValidation;

namespace EShop.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Category name is required.")
            .Must(name => name.Trim().Length <= CategoryConstraints.NameMaxLength)
            .WithMessage($"Category name must be at most {CategoryConstraints.NameMaxLength} characters.");
    }
}
