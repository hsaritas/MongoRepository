using MongoRepository.Entities.Abstract;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoRepository.DataAccess.Abstract
{
    public abstract class MongoDbBsonRepositoryBase<T>
    {
        protected IMongoCollection<T> Collection;
        protected IMongoDatabase Database;

        protected MongoDbBsonRepositoryBase(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetSection("AppSettings:MongoDbSettings:ConnectionString").Value);
            Database = client.GetDatabase(configuration.GetSection("AppSettings:MongoDbSettings:Database").Value);
            this.Collection = Database.GetCollection<T>(configuration.GetSection("AppSettings:MongoDbSettings:Collection").Value);
        }

        public void SetCollection(string collectionName)
        {
            this.Collection = Database.GetCollection<T>(collectionName);
        }

        public long Count(FilterDefinition<T> filter)
        {
            return Collection.CountDocuments(filter);
        }

        public IEnumerable<T> Search(FilterDefinition<T> filter)
        {
            return Collection.Find(filter).ToEnumerable<T>();
        }

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null
                ? Collection.AsQueryable()
                : Collection.AsQueryable().Where(predicate);
        }  

        public virtual Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return Collection.Find(predicate).FirstOrDefaultAsync();
        }

        public virtual Task<T> GetByIdAsync(string id)
        {
            var filter = new BsonDocument { { "_id", id } };
            return Collection.Find(filter).SingleAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            var options = new InsertOneOptions {BypassDocumentValidation = false};
            await Collection.InsertOneAsync(entity, options);
            return entity;
        }

        public virtual async Task<bool> AddRangeAsync(IEnumerable<T> entities)
        {
            var options = new BulkWriteOptions {IsOrdered = false, BypassDocumentValidation = false};
            return (await Collection.BulkWriteAsync((IEnumerable<WriteModel<T>>) entities, options)).IsAcknowledged;
        }

        public virtual async Task<T> UpdateAsync(string id, T entity)
        {
            var filter = new BsonDocument { { "_id", id } };
            return await Collection.FindOneAndReplaceAsync(filter, entity);
        }

        public virtual async Task<T> UpdateAsync(T entity, Expression<Func<T, bool>> predicate)
        {
            return await Collection.FindOneAndReplaceAsync(predicate, entity);
        }
        public virtual async Task<T> DeleteAsync(string id)
        {
            var filter = new BsonDocument { { "_id", id } };
            return await Collection.FindOneAndDeleteAsync(filter);
        }

        public virtual async Task<T> DeleteAsync(Expression<Func<T, bool>> filter)
        {
            return await Collection.FindOneAndDeleteAsync(filter);
        }
    }
}