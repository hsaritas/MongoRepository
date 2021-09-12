using MongoRepository.DataAccess.Concrete;
using MongoRepository.Models;
using MongoRepository.Services.Abstract;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoRepository.Services.Concrete
{
    public class MongoBsonRepositoryService : IMongoBsonRepositoryService
    {
        private readonly MongoDbBsonDal dsLogMongoDbBsonDal;
        public MongoBsonRepositoryService(MongoDbBsonDal dsLogMongoDbBsonDal)
        {
            this.dsLogMongoDbBsonDal = dsLogMongoDbBsonDal;
        }
        public async Task<ApiResponse<PagedResult<BsonDocument>>> SearchBsonAsync(List<KeyValuePair<string, string>> model)
        {
            var builder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filters = builder.Where(x => true);

            var pageNumber = 0;
            var pageSize = 20;
            foreach (var kv in model)
            {
                if (kv.Key.ToLowerInvariant() == "pagenumber")
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
                        filters = new BsonDocument { { kv.Key, bool.Parse(kv.Value) } };
                    else if (long.TryParse(kv.Value, out _))
                        filters &= new BsonDocument { { kv.Key, long.Parse(kv.Value) } };
                    else
                        filters = new BsonDocument { { kv.Key, kv.Value } };
                }
            }

            var result = dsLogMongoDbBsonDal.Search(filters);
            var count = dsLogMongoDbBsonDal.Count(filters);
            var pagedResult = PagedResult<BsonDocument>.ToPagedList(result, count, pageNumber, pageSize);
            return await ApiResponseFactory.Success(pagedResult);
        }
    }
}
