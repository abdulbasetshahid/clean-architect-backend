using MediatR;

namespace EShop.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public required string Name { get; set; }
}
