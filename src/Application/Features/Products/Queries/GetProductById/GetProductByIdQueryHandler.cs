using AutoMapper;
using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Domain.Entities;
using MediatR;

namespace EShop.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDetailVm>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductDetailVm> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(request.Id, cancellationToken);
        if (product is null)
            throw new NotFoundException(nameof(Product), request.Id);

        return _mapper.Map<ProductDetailVm>(product);
    }
}
