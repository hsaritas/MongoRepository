using MongoRepository.DataAccess.Abstract;
using MongoRepository.Entities.Concrete;
using MongoRepository.Models;
using MongoRepository.Services.Abstract;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoRepository.Services.Concrete
{
    public class MongoModelRepositoryService : IMongoModelRepositoryService
    {
        private readonly IMongodbDal dsLogMongoDbDal;
        public MongoModelRepositoryService(IMongodbDal dsLogMongoDbDal)
        {
            this.dsLogMongoDbDal = dsLogMongoDbDal;
        }

        public async Task<ApiResponse<PagedResult<MongoDbEntity>>> SetCollection(string url, string dbname, string collection)
        {
            if (!string.IsNullOrEmpty(url)
                || !string.IsNullOrEmpty(dbname)
                || !string.IsNullOrEmpty(collection))
            {
                if (!string.IsNullOrEmpty(url)
                && !string.IsNullOrEmpty(dbname)
                && !string.IsNullOrEmpty(collection))
                {
                    dsLogMongoDbDal.SetCollection(url, dbname, collection);
                    return await ApiResponseFactory.Success<PagedResult<MongoDbEntity>>(null);
                }
                else
                {
                    return await ApiResponseFactory
                            .Error<PagedResult<MongoDbEntity>>(
                                new string[] {
                                    "MongoDatabaseUrl, MongoDatabaseName and MongoCollectionName paremeters must not be empty"
                                });
                }
            }
            return await ApiResponseFactory.Success<PagedResult<MongoDbEntity>>(null);
        }

        public async Task<ApiResponse<PagedResult<MongoDbEntity>>> SearchAsync(SearchModel model)
        {
            var builder = Builders<MongoDbEntity>.Filter;
            FilterDefinition<MongoDbEntity> filters = builder.Where(x => true);

            var coll = await SetCollection(model.MongoDatabaseUrl, model.MongoDatabaseName, model.MongoCollectionName);
            if(coll.Success == false)
            {
                return coll;
            }

            if (!string.IsNullOrEmpty(model.Id))
            {
                filters &= builder.Where(x => x.Id == model.Id);
            }
            if (model.StartDate.HasValue)
            {
                filters &= builder.Where(x => x.Timestamp >= model.StartDate.Value);
            }
            if (model.EndDate.HasValue)
            {
                filters &= builder.Where(x => x.Timestamp <= model.EndDate.Value);
            }
            
            var result = dsLogMongoDbDal.Search(filters);
            var count = dsLogMongoDbDal.Count(filters);
            var pagedResult = PagedResult<MongoDbEntity>.ToPagedList(result, count, model.PageNumber, model.PageSize);
            return await ApiResponseFactory.Success<PagedResult<MongoDbEntity>>(pagedResult);
        }

        public async Task<ApiResponse<PagedResult<MongoDbEntity>>> SearchAsync(List<KeyValuePair<string, string>> model)
        {
            var builder = Builders<MongoDbEntity>.Filter;
            FilterDefinition<MongoDbEntity> filters = builder.Where(x => true);
            var pageNumber = 0;
            var pageSize = 20;
            string databaseUrl = null;
            string databaseName = null;
            string collectionName = null;
            foreach (var kv in model)
            {
                if (kv.Key.ToLowerInvariant() == "mongodatabaseurl")
                {
                    databaseUrl = kv.Value;
                }
                else if (kv.Key.ToLowerInvariant() == "mongodatabasename")
                {
                    databaseName = kv.Value;
                }
                else if (kv.Key.ToLowerInvariant() == "mongocollectionname")
                {
                    collectionName = kv.Value;
                }
                else if (kv.Key.ToLowerInvariant() == "pagenumber")
                {
                    pageNumber = int.Parse(kv.Value);
                }
                else if (kv.Key.ToLowerInvariant() == "pagesize")
                {
                    pageSize = int.Parse(kv.Value);
                }
                else if (kv.Key.ToLowerInvariant() == "startdate")
                {
                    filters &= builder.Gte("Timestamp", kv.Value);
                }
                else if (kv.Key.ToLowerInvariant() == "enddate")
                {
                    filters &= builder.Lte("Timestamp", kv.Value);
                }
                else
                {
                    if (bool.TryParse(kv.Value, out _))
                        filters &= builder.Eq(kv.Key, bool.Parse(kv.Value));
                    else if (long.TryParse(kv.Value, out _))
                        filters &= builder.Eq(kv.Key, long.Parse(kv.Value));
                    else
                        filters &= builder.Eq(kv.Key, kv.Value);
                }
            }
            var collResult = await SetCollection(databaseUrl, databaseName, collectionName);
            if (collResult.Success == false)
            {
                return collResult;
            }
            var result = dsLogMongoDbDal.Search(filters);
            var count = dsLogMongoDbDal.Count(filters);
            var pagedResult = PagedResult<MongoDbEntity>.ToPagedList(result, count, pageNumber, pageSize);
            return await ApiResponseFactory.Success(pagedResult);
        }
        public async Task<ApiResponse<bool>> Update(MongoDbEntity model)
        {
            var updated = await dsLogMongoDbDal.UpdateAsync(model.Id, model);
            return await ApiResponseFactory.Success<bool>(true);
        }


    }
}
