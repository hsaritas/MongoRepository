using MongoRepository.Entities.Concrete;
using MongoRepository.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoRepository.Services.Abstract
{
    public interface IMongoModelRepositoryService
    {
        Task<ApiResponse<PagedResult<MongoDbEntity>>> SetCollection(string url, string dbname, string collection);
        Task<ApiResponse<PagedResult<MongoDbEntity>>> SearchAsync(SearchModel model);
        Task<ApiResponse<PagedResult<MongoDbEntity>>> SearchAsync(List<KeyValuePair<string, string>> lst);
        Task<ApiResponse<bool>> Update(MongoDbEntity model);
    }
}
