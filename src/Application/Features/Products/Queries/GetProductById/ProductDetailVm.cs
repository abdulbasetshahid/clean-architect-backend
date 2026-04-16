namespace EShop.Application.Features.Products.Queries.GetProductById;

public class ProductDetailVm
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool InStock { get; set; }
    public bool IsBestSeller { get; set; }
    public string? ImageUrl { get; set; }
    public Guid CategoryId { get; set; }
    public required string CategoryName { get; set; }
}
