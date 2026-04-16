using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Domain.Entities;
using MediatR;

namespace EShop.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Unit>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id);
        if (category is null)
            throw new NotFoundException(nameof(Category), request.Id);

        category.Name = request.Name.Trim();
        await _categoryRepository.UpdateAsync(category);

        return Unit.Value;
    }
}
