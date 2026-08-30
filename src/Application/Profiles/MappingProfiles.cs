using AutoMapper;
using EShop.Application.Features.Categories.Queries.GetCategories;
using EShop.Application.Features.Products.Queries.GetProductById;
using EShop.Application.Features.Products.Queries.GetProducts;
using EShop.Domain.Entities;

namespace EShop.Application.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Category, CategoryListVm>().ReverseMap();

        CreateMap<Product, ProductListVm>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.Price, o => o.MapFrom(s => s.ProductVariants
                .Select(d => (decimal?)d.Price)
                .FirstOrDefault() ?? 0m))
            .ForMember(d => d.InStock, o => o.MapFrom(s => s.ProductVariants
                .Select(d => (bool?)d.InStock)
                .FirstOrDefault() ?? false));

        CreateMap<Product, ProductDetailVm>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.Description, o => o.MapFrom(s => s.ProductVariants
                .Select(d => d.Description)
                .FirstOrDefault()))
            .ForMember(d => d.Price, o => o.MapFrom(s => s.ProductVariants
                .Select(d => (decimal?)d.Price)
                .FirstOrDefault() ?? 0m))
            .ForMember(d => d.InStock, o => o.MapFrom(s => s.ProductVariants
                .Select(d => (bool?)d.InStock)
                .FirstOrDefault() ?? false));
    }
}
