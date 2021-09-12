using MongoRepository.Models;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoRepository.Services.Abstract
{
    public interface IMongoBsonRepositoryService
    {
        Task<ApiResponse<PagedResult<BsonDocument>>> SearchBsonAsync(List<KeyValuePair<string, string>> model);
    }
}
