using AutoMapper;
using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Application.Features.Categories.Queries.GetCategories;
using EShop.Domain.Entities;
using MediatR;

namespace EShop.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryListVm>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryListVm> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id);
        if (category is null)
            throw new NotFoundException(nameof(Category), request.Id);

        return _mapper.Map<CategoryListVm>(category);
    }
}
