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

        public long Count()
        {
            return Objects.CountDocuments(FilterDefinition<TEntity>.Empty);
        }

        public TEntity Add(TEntity entity)
        {
            Objects.InsertOne(entity);
            return GetObjectByID(entity.ID);
        }

        public TEntity Update(TEntity entity)
        {
            TEntity t = GetObjectByID(entity.ID);
            if (t == null)
                return null;
            var mapper = MapperFactory.Map<TEntity, TEntity>();
            t = mapper.Map<TEntity, TEntity>(entity);
            var updateResult = Objects.ReplaceOne(
                o => o.ID == t.ID, replacement: entity);
            if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                return GetObjectByID(entity.ID);
            return null;
        }

        public bool Delete(int id)
        {
            var deleteResult = Objects.DeleteOne(o => o.ID == id);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public TEntity GetObjectByID(int id)
        {
            var builder = Builders<TEntity>.Filter;
            var filter = builder.Eq("ID", id);
            return Objects.Find(filter).FirstOrDefault();
        }

        public IEnumerable<TEntity> Get(int limit, int start)
        {
            return Objects.Find(FilterDefinition<TEntity>.Empty)
                .Skip(start)
                .Limit(limit).ToList();
        }
    }
}