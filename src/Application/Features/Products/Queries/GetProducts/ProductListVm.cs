using EShop.Application.Features.Products.Queries;

namespace EShop.Application.Features.Products.Queries.GetProducts;

public class ProductListVm
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public bool InStock { get; set; }
    public bool IsBestSeller { get; set; }
    public string? ImageUrl { get; set; }
    public Guid CategoryId { get; set; }
    public required string CategoryName { get; set; }
    public IReadOnlyList<ProductVariantVm> Variants { get; set; } = [];
}
