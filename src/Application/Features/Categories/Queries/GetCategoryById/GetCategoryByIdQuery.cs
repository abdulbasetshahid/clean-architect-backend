using EShop.Application.Features.Categories.Queries.GetCategories;
using MediatR;

namespace EShop.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQuery : IRequest<CategoryListVm>
{
    public Guid Id { get; }

    public GetCategoryByIdQuery(Guid id) => Id = id;
}
