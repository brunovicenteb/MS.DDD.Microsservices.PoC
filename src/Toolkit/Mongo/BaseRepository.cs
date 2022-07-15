using MongoDB.Bson;
using Toolkit.Data;
using MongoDB.Driver;
using Toolkit.Interfaces;
using MongoDB.Bson.Serialization;
using Toolkit.Mapper;

namespace Toolkit.Mongo
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        static BaseRepository()
        {
            BsonClassMap.RegisterClassMap<BaseEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdMember(c => c.ID);
            });
        }

        public BaseRepository()
        {
            var client = new MongoClient(TransactionalOutboxEnvironment.StringConnection);
            var database = client.GetDatabase(TransactionalOutboxEnvironment.DataBaseName);
            Objects = database.GetCollection<TEntity>(CollectionName);
        }

        protected readonly IMongoCollection<TEntity> Objects;
        protected abstract string CollectionName { get; }

        public async Task<long> CountAsync()
        {
            return await Objects.CountDocumentsAsync(FilterDefinition<TEntity>.Empty);
        }

        public async Task<TEntity> AddAsync(TEntity entity, bool applySave = true)
        {
            await Objects.InsertOneAsync(entity);
            return await GetObjectByIDAsync(entity.ID);
        }
            
        public async Task<TEntity> UpdateAsync(TEntity entity, bool applySave = true)
        {
            TEntity t = await GetObjectByIDAsync(entity.ID);
            if (t == null)
                return null;
            var mapper = MapperFactory.Map<TEntity, TEntity>();
            t = mapper.Map<TEntity, TEntity>(entity);
            var updateResult = await Objects.ReplaceOneAsync(
                o => o.ID == t.ID, replacement: entity);
            if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                return await GetObjectByIDAsync(entity.ID);
            return null;
        }

        public async Task<bool> DeleteAsync(int id, bool applySave = true)
        {
            var deleteResult = await Objects.DeleteOneAsync(o => o.ID == id);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<TEntity> GetObjectByIDAsync(int id)
        {
            var builder = Builders<TEntity>.Filter;
            var filter = builder.Eq("ID", id);
            return await Objects.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAsync(int limit, int start)
        {
            return await Objects.Find(FilterDefinition<TEntity>.Empty)
                .Skip(start)
                .Limit(limit).ToListAsync();
        }
    }
}