using Toolkit.Data;

namespace Toolkit.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<long> CountAsync();
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(int id);
    Task<TEntity> GetObjectByIDAsync(int id);
    Task<IEnumerable<TEntity>> GetAsync(int limit, int start);
}