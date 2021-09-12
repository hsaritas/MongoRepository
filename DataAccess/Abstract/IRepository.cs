using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoRepository.Entities.Abstract;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoRepository.DataAccess.Abstract
{
    public interface IRepository<T, in TKey> where T : class, IEntity<TKey>, new() where TKey : IEquatable<TKey>
    {
        void SetCollection(string databaseUrl, string databaseName, string collectionName);
        long Count(FilterDefinition<T> filter);
        IEnumerable<T> Search(FilterDefinition<T> filter);
        long CountBson(FilterDefinition<BsonDocument> filter);
        IEnumerable<BsonDocument> SearchBson(FilterDefinition<BsonDocument> filter);
        IQueryable<T> Get(Expression<Func<T, bool>> predicate = null);
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetByIdAsync(TKey id);
        Task<T> AddAsync(T entity);
        Task<bool> AddRangeAsync(IEnumerable<T> entities);
        Task<T> UpdateAsync(TKey id, T entity);
        Task<T> UpdateAsync(T entity, Expression<Func<T, bool>> predicate);
        Task<T> DeleteAsync(T entity);
        Task<T> DeleteAsync(TKey id);
        Task<T> DeleteAsync(Expression<Func<T, bool>> filter);
    }
}