namespace EShop.Application.Features.Products.Queries;

public class ProductVariantVm
{
    public Guid Id { get; set; }
    public required string VariationName { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool InStock { get; set; }
    public bool IsActive { get; set; }
}
