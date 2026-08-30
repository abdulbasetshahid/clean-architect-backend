namespace EShop.Application.Features.Products;

public class ProductVariantDto
{
    public Guid? Id { get; set; }
    public required string VariationName { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool InStock { get; set; } = true;
    public bool IsActive { get; set; } = true;
}
