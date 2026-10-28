using EShop.Application.Features.Products;
using MediatR;

namespace EShop.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<Guid>
{
    public required string Name { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool InStock { get; set; } = true;
    public bool IsBestSeller { get; set; }
    public string? ImageUrl { get; set; }
    public Guid CategoryId { get; set; }

    public IReadOnlyList<ProductVariantDto>? Variants { get; set; }
}
