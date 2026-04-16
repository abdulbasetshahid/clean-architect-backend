using MediatR;

namespace EShop.Application.Features.Products.Queries.GetProducts;

public class GetProductListQuery : IRequest<List<ProductListVm>>
{
}
