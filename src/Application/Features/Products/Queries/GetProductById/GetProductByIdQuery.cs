using MediatR;

namespace EShop.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ProductDetailVm>
{
    public Guid Id { get; }

    public GetProductByIdQuery(Guid id) => Id = id;
}
