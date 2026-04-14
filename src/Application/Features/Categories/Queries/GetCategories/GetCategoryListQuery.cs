using MediatR;

namespace EShop.Application.Features.Categories.Queries.GetCategories
{
    public class GetCategoryListQuery : IRequest<List<CategoryListVm>>
    {
    }
}
