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

    protected readonly TContext Context;

    protected abstract DbSet<TEntity> Collection { get; }

    public long Count()
    {
        return Collection.Count();
    }

    public bool Delete(int id)
    {
        TEntity entity = GetObjectByID(id);
        Collection.Remove(entity);
        return Context.SaveChanges() == 1;
    }

    public TEntity GetObjectByID(int id)
    {
        return Collection
            .AsNoTracking()
            .FirstOrDefault(o => o.ID == id);
    }

    public IEnumerable<TEntity> Get(int limit, int start)
    {
        return Collection
            .Skip(limit)
            .Take(start)
            .AsNoTracking()
            .ToList();
    }

    public TEntity Add(TEntity entity)
    {
        Collection.Add(entity);
        Context.SaveChanges();
        return GetObjectByID(entity.ID);
    }

    public TEntity Update(TEntity entity)
    {
        Collection.Update(entity);
        Context.SaveChanges();
        return GetObjectByID(entity.ID);
    }
}