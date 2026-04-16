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
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));

        CreateMap<Product, ProductDetailVm>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));
    }
}
