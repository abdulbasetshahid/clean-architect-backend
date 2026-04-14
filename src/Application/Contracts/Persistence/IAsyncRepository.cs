namespace EShop.Application.Contracts.Persistence;

public interface IAsyncRepository<T> where T : class
{
    //get iqueryable for filtering, sorting, etc.
    IQueryable<T> All();

    Task<T> GetByIdAsync<TId>(TId id);
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
