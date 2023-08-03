using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq.Expressions;
using userMS.Application.Repositories;
using userMS.Domain.Entities.Common;
using userMS.Persistence.Data;

namespace userMS.Persistence.Repositories
{
    public class Repository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : IEntity<TPrimaryKey>
    {
        private readonly IOptions<DatabaseSettings> _dbSettings;
        private readonly IMongoCollection<TEntity> _collection;

        public Repository(IOptions<DatabaseSettings> dbSettings)
        {
            _dbSettings = dbSettings;

            // mongo client is responsible for managing the connection to the MongoDB server
            var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _collection = mongoDatabase.GetCollection<TEntity>(typeof(TEntity).Name + "Collection");
        }

        public TEntity Add(TEntity entity)
        {
            _collection.InsertOne(entity);
            return entity;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            _collection.InsertMany(entities);
            return entities;
        }

        public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _collection.InsertManyAsync(entities);
            return entities;
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return _collection.Find(predicate).Any();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _collection.Find(predicate).AnyAsync();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return (int)_collection.CountDocuments(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return (int)await _collection.CountDocumentsAsync(predicate);
        }

        public bool Delete(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);
            var deleteResult = _collection.DeleteOne(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<bool> DeleteAsync(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);
            var deleteResult = await _collection.DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public bool DeleteById(TPrimaryKey id)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id);
            var deleteResult = _collection.DeleteOne(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<bool> DeleteByIdAsync(TPrimaryKey id)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id);
            var deleteResult = await _collection.DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public bool DeleteRange(IEnumerable<TEntity> entities)
        {
            var idList = entities.Select(e => e.Id).ToList();
            var filter = Builders<TEntity>.Filter.In("_id", idList);
            var deleteResult = _collection.DeleteMany(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            var idList = entities.Select(e => e.Id).ToList();
            var filter = Builders<TEntity>.Filter.In(e => e.Id, idList);
            var deleteResult = await _collection.DeleteManyAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return _collection.Find(predicate).ToList();
        }

        public async Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _collection.Find(_ => true).ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public TEntity GetById(TPrimaryKey id)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id);

            return _collection.Find(filter).FirstOrDefault();
        }

        public async Task<TEntity> GetByIdAsync(TPrimaryKey id)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id);

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public TEntity Update(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);
            var updateResult = _collection.ReplaceOne(filter, entity);

            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);
            var updateResult = await _collection.ReplaceOneAsync(filter, entity);

            return entity;
        }

        public IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {
                var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);
                var updateResult = _collection.ReplaceOne(filter, entity);
            }

            return entities;
        }

        public async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {

            foreach (TEntity entity in entities)
            {
                var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);
                var updateResult = await _collection.ReplaceOneAsync(filter, entity);
            }

            return entities;
        }

    }
}
