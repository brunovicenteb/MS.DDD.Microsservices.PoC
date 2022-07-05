﻿using Toolkit.Data;

namespace Toolkit.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    long Count();
    TEntity Add(TEntity entity);
    TEntity Update(TEntity entity);
    bool Delete(string id);
    TEntity GetObjectByID(string id);
    IEnumerable<TEntity> Get(int limit, int start);
}