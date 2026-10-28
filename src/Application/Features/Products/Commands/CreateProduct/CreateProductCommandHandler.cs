using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Domain.Entities;
using MediatR;

namespace EShop.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _ = await _categoryRepository.GetByIdAsync(request.CategoryId)
            ?? throw new NotFoundException(nameof(Category), request.CategoryId);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            ShortDescription = NormalizeOptional(request.ShortDescription),
            IsBestSeller = request.IsBestSeller,
            ImageUrl = NormalizeOptional(request.ImageUrl),
            CategoryId = request.CategoryId,
            ProductVariants = BuildVariants(request)
        };

        await _productRepository.AddAsync(product);
        return product.Id;
    }

    private static List<ProductVariant> BuildVariants(CreateProductCommand request)
    {
        if (request.Variants is { Count: > 0 })
        {
            return request.Variants.Select(v => new ProductVariant
            {
                Id = Guid.NewGuid(),
                VariationName = v.VariationName.Trim(),
                Description = NormalizeOptional(v.Description),
                Price = v.Price,
                InStock = v.InStock,
                IsActive = v.IsActive
            }).ToList();
        }

        return
        [
            new ProductVariant
            {
                Id = Guid.NewGuid(),
                VariationName = request.Name.Trim(),
                Description = NormalizeOptional(request.Description),
                Price = request.Price,
                InStock = request.InStock,
                IsActive = true
            }
        ];
    }

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var t = value.Trim();
        return t.Length == 0 ? null : t;
    }
}
