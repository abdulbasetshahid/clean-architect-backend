using EShop.Application.Features.Categories;
using FluentValidation;

namespace EShop.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Category id is required.");

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Category name is required.")
            .Must(name => name.Trim().Length <= CategoryConstraints.NameMaxLength)
            .WithMessage($"Category name must be at most {CategoryConstraints.NameMaxLength} characters.");
    }
}
