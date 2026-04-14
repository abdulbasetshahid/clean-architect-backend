using AutoMapper;
using EShop.Application.Features.Categories.Commands.CreateCategory;
using EShop.Application.Features.Categories.Queries.GetCategories;
using EShop.Domain.Entities;

namespace EShop.Application.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Category, CategoryListVm>().ReverseMap();

        CreateMap<Category, CreateCategoryCommand>().ReverseMap();
    }
}
