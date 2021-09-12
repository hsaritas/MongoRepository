using MongoRepository.Entities.Concrete;
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
    public abstract class MongoDbRepositoryBase<T> : IRepository<T, string> where T : MongoDbEntity, new()
    {
        protected IMongoCollection<T> Collection;
        protected IMongoCollection<BsonDocument> bsonCollection;

        protected MongoDbRepositoryBase(IConfiguration configuration)
        {
            var settings = new MongoClientSettings
            {
                Server = new MongoServerAddress(configuration.GetSection("AppSettings:MongoDbSettings:Server").Value, 27017),
                SocketTimeout = new TimeSpan(0, int.Parse(configuration.GetSection("AppSettings:MongoDbSettings:TimeoutInMinutes").Value), 0),
                WaitQueueTimeout = new TimeSpan(0, int.Parse(configuration.GetSection("AppSettings:MongoDbSettings:TimeoutInMinutes").Value), 0),
                ConnectTimeout = new TimeSpan(0, int.Parse(configuration.GetSection("AppSettings:MongoDbSettings:TimeoutInMinutes").Value), 0), 
            };            
            var client = new MongoClient(settings);
            var database = client.GetDatabase(configuration.GetSection("AppSettings:MongoDbSettings:Database").Value);
            this.Collection = database.GetCollection<T>(configuration.GetSection("AppSettings:MongoDbSettings:Collection").Value);
            this.bsonCollection = database.GetCollection<BsonDocument>(configuration.GetSection("AppSettings:MongoDbSettings:Collection").Value);
        }

        public void SetCollection(string databaseUrl, string databaseName, string collectionName)
        {
            var client = new MongoClient(databaseUrl);
            var database = client.GetDatabase(databaseName);
            this.Collection = database.GetCollection<T>(collectionName);
        }

        public long Count(FilterDefinition<T> filter)
        {
            return Collection.CountDocuments(filter);
        }

        public IEnumerable<T> Search(FilterDefinition<T> filter)
        {
            return Collection.Find(filter).ToEnumerable<T>();
        }

        public IEnumerable<BsonDocument> SearchBson(FilterDefinition<BsonDocument> filter)
        {
            return bsonCollection.Find<BsonDocument>(filter).ToEnumerable<BsonDocument>();
        }
        public long CountBson(FilterDefinition<BsonDocument> filter)
        {
            return bsonCollection.CountDocuments(filter);
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
            return Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
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
            return await Collection.FindOneAndReplaceAsync(x => x.Id == id, entity);
        }

        public virtual async Task<T> UpdateAsync(T entity, Expression<Func<T, bool>> predicate)
        {
            return await Collection.FindOneAndReplaceAsync(predicate, entity);
        }

        public virtual async Task<T> DeleteAsync(T entity)
        {
            return await Collection.FindOneAndDeleteAsync(x => x.Id == entity.Id);
        }

        public virtual async Task<T> DeleteAsync(string id)
        {
            return await Collection.FindOneAndDeleteAsync(x => x.Id == id);
        }

        public virtual async Task<T> DeleteAsync(Expression<Func<T, bool>> filter)
        {
            return await Collection.FindOneAndDeleteAsync(filter);
        }
    }
}