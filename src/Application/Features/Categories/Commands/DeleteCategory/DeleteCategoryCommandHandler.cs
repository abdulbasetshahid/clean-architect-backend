using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Domain.Entities;
using MediatR;

namespace EShop.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id);
        if (category is null)
            throw new NotFoundException(nameof(Category), request.Id);

        if (await _categoryRepository.HasProductsInCategoryAsync(request.Id, cancellationToken))
            throw new BadRequestException("Cannot delete a category that still has products.");

        await _categoryRepository.DeleteAsync(category);

        return Unit.Value;
    }
}
