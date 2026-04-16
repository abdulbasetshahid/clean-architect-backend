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
            Description = NormalizeOptional(request.Description),
            Price = request.Price,
            InStock = request.InStock,
            IsBestSeller = request.IsBestSeller,
            ImageUrl = NormalizeOptional(request.ImageUrl),
            CategoryId = request.CategoryId
        };

        await _productRepository.AddAsync(product);
        return product.Id;
    }

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var t = value.Trim();
        return t.Length == 0 ? null : t;
    }
}
