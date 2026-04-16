using MediatR;

namespace EShop.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommand : IRequest<Unit>
{
    public Guid Id { get; }

    public DeleteCategoryCommand(Guid id) => Id = id;
}
