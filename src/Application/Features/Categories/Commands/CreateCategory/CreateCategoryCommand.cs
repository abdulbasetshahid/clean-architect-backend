using MediatR;

namespace EShop.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<Guid>
{
    public required string Name { get; set; }
}
