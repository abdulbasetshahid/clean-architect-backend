using Domain.Entities;

namespace Application.Contracts.Persistence
{
    public interface ICategoryRepository: IAsyncRepository<Category>
    {
    }
}
