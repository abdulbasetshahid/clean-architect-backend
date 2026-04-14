using EShop.Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EShop.Persistence.Repositories.Common;

public class BaseRepository<T> : IAsyncRepository<T> where T : class
{
    protected readonly EShopDbContext _dbContext;

    public BaseRepository(EShopDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public virtual async Task<T> GetByIdAsync<TId>(TId id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public virtual async Task<IReadOnlyList<T>> ListAllAsync()
    {
        return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
    }
    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }
    public virtual async Task UpdateAsync(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }
    public virtual async Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public IQueryable<T> All()
    {
        return _dbContext.Set<T>();
    }
}
