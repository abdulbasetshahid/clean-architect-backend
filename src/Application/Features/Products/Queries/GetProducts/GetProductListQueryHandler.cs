using AutoMapper;
using EShop.Application.Contracts.Persistence;
using MediatR;

namespace EShop.Application.Features.Products.Queries.GetProducts;

public class GetProductListQueryHandler : IRequestHandler<GetProductListQuery, List<ProductListVm>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductListQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<List<ProductListVm>> Handle(GetProductListQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.ListWithCategoryAsync(cancellationToken);
        return _mapper.Map<List<ProductListVm>>(products);
    }
}
