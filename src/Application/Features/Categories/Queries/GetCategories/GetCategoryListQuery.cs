using MediatR;

namespace Application.Features.Categories.Queries.GetCategories
{
    public class GetCategoryListQuery : IRequest<List<CategoryListVm>>
    {
    }
}
