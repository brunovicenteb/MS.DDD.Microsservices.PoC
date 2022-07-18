using Toolkit.Data;
using Toolkit.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Toolkit.RelationalDb;

public abstract class RelationalDbRepository<TContext, TEntity> : IBaseRepository<TEntity>
    where TContext : DbContext
    where TEntity : BaseEntity
{
    public RelationalDbRepository(TContext context)
    {
        Context = context;
    }

    public readonly TContext Context;
    protected abstract DbSet<TEntity> Collection { get; }

    public async Task<long> CountAsync()
    {
        return await Collection.CountAsync();
    }

    public async Task<bool> DeleteAsync(int id, bool applySave = true)
    {
        TEntity entity = await GetObjectByIDAsync(id);
        Collection.Remove(entity);
        if (!applySave)
            return true;
        return await Context.SaveChangesAsync() == 1;
    }

    public async Task<TEntity> GetObjectByIDAsync(int id)
    {
        return await Collection
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.ID == id);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAsync(int limit, int start)
    {
        return await Collection
            .Skip(limit)
            .Take(start)
            .OrderBy(o => o.ID)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity, bool applySave = true)
    {
        await Collection.AddAsync(entity);
        if (applySave)
            await Context.SaveChangesAsync();
        return await GetObjectByIDAsync(entity.ID);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, bool applySave = true)
    {
        Collection.Update(entity);
        if (applySave)
            await Context.SaveChangesAsync();
        return await GetObjectByIDAsync(entity.ID);
    }
}