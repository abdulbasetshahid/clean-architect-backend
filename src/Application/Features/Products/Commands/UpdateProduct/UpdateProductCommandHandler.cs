using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Application.Features.Products;
using EShop.Domain.Entities;
using MediatR;

namespace EShop.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithDetailsAsync(
            request.Id,
            asNoTracking: false,
            cancellationToken);
        if (product is null)
            throw new NotFoundException(nameof(Product), request.Id);

        _ = await _categoryRepository.GetByIdAsync(request.CategoryId)
            ?? throw new NotFoundException(nameof(Category), request.CategoryId);

        var validator = new UpdateProductCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult);

        product.Name = request.Name.Trim();
        product.ShortDescription = NormalizeOptional(request.ShortDescription);
        product.IsBestSeller = request.IsBestSeller;
        product.ImageUrl = NormalizeOptional(request.ImageUrl);
        product.CategoryId = request.CategoryId;

        if (request.Variants is { Count: > 0 })
            SyncVariants(product, request.Variants);
        else
            UpdateDefaultVariant(product, request);

        await _productRepository.UpdateAsync(product);
        return Unit.Value;
    }

    private static void SyncVariants(Product product, IReadOnlyList<ProductVariantDto> variants)
    {
        var incomingIds = variants
            .Where(v => v.Id.HasValue && v.Id.Value != Guid.Empty)
            .Select(v => v.Id!.Value)
            .ToHashSet();

        foreach (var existing in product.ProductVariants)
        {
            if (!incomingIds.Contains(existing.Id))
                existing.IsActive = false;
        }

        foreach (var input in variants)
        {
            if (input.Id is Guid id && id != Guid.Empty)
            {
                var existing = product.ProductVariants.FirstOrDefault(v => v.Id == id)
                    ?? throw new NotFoundException(nameof(ProductVariant), id);

                existing.VariationName = input.VariationName.Trim();
                existing.Description = NormalizeOptional(input.Description);
                existing.Price = input.Price;
                existing.InStock = input.InStock;
                existing.IsActive = input.IsActive;
            }
            else
            {
                product.ProductVariants.Add(new ProductVariant
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    VariationName = input.VariationName.Trim(),
                    Description = NormalizeOptional(input.Description),
                    Price = input.Price,
                    InStock = input.InStock,
                    IsActive = input.IsActive
                });
            }
        }
    }

    private static void UpdateDefaultVariant(Product product, UpdateProductCommand request)
    {
        var detail = product.ProductVariants.FirstOrDefault();
        if (detail is null)
        {
            detail = new ProductVariant
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                VariationName = product.Name,
                IsActive = true
            };
            product.ProductVariants.Add(detail);
        }

        detail.VariationName = product.Name;
        detail.Description = NormalizeOptional(request.Description);
        detail.Price = request.Price;
        detail.InStock = request.InStock;
        detail.IsActive = true;
    }

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var t = value.Trim();
        return t.Length == 0 ? null : t;
    }
}
