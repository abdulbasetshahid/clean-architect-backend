using MediatR;

namespace Application.Features.Queries.GetCategories
{
    public class GetCategoryListQuery : IRequest<List<CategoryListVm>>
    {
    }
}
