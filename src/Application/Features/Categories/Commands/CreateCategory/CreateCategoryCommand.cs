using MediatR;

namespace EShop.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommand : IRequest<Guid>
    {
        public int Id { get; set; }

        public required string Name { get; set; }
    }
}
