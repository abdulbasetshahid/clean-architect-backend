using AutoMapper;
using EShop.Application.Features.Categories.Queries.GetCategories;
using EShop.Application.Features.Products.Queries;
using EShop.Application.Features.Products.Queries.GetProductById;
using EShop.Application.Features.Products.Queries.GetProducts;
using EShop.Domain.Entities;

namespace EShop.Application.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Category, CategoryListVm>().ReverseMap();

        CreateMap<ProductVariant, ProductVariantVm>();

        CreateMap<Product, ProductListVm>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.Price, o => o.MapFrom(s => PrimaryVariant(s) != null ? PrimaryVariant(s)!.Price : 0m))
            .ForMember(d => d.InStock, o => o.MapFrom(s => PrimaryVariant(s) != null && PrimaryVariant(s)!.InStock))
            .ForMember(d => d.Variants, o => o.MapFrom(s => s.ProductVariants));

        CreateMap<Product, ProductDetailVm>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.Description, o => o.MapFrom(s => PrimaryVariant(s) != null ? PrimaryVariant(s)!.Description : null))
            .ForMember(d => d.Price, o => o.MapFrom(s => PrimaryVariant(s) != null ? PrimaryVariant(s)!.Price : 0m))
            .ForMember(d => d.InStock, o => o.MapFrom(s => PrimaryVariant(s) != null && PrimaryVariant(s)!.InStock))
            .ForMember(d => d.Variants, o => o.MapFrom(s => s.ProductVariants));
    }

    private static ProductVariant? PrimaryVariant(Product product) =>
        product.ProductVariants.FirstOrDefault(v => v.IsActive)
        ?? product.ProductVariants.FirstOrDefault();
}
