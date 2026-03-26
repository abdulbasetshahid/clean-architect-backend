using Application.Features.Categories.Commands.CreateCategory;
using Application.Features.Categories.Queries.GetCategories;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Category, CategoryListVm>().ReverseMap();

        CreateMap<Category, CreateCategoryCommand>().ReverseMap();
    }
}
