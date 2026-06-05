using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
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

        var detail = product.ProductDetails.FirstOrDefault();
        if (detail is null)
        {
            detail = new ProductDetail
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                VariationName = product.Name
            };
            product.ProductDetails.Add(detail);
        }

        detail.VariationName = product.Name;
        detail.Description = NormalizeOptional(request.Description);
        detail.Price = request.Price;
        detail.InStock = request.InStock;

        await _productRepository.UpdateAsync(product);
        return Unit.Value;
    }

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var t = value.Trim();
        return t.Length == 0 ? null : t;
    }
}
