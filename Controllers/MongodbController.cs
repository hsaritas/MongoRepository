using MongoRepository.Entities.Concrete;
using MongoRepository.Models;
using MongoRepository.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoRepository.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MongodbController : ControllerBase
    {
        private readonly IMongoModelRepositoryService _mongoRepositoryService;
        private readonly IMongoBsonRepositoryService _mongoBsonRepositoryService;

        public MongodbController(IMongoModelRepositoryService mongoRepositoryService,
            IMongoBsonRepositoryService mongoBsonRepositoryService)
        {
            this._mongoRepositoryService = mongoRepositoryService;
            this._mongoBsonRepositoryService = mongoBsonRepositoryService;
        }
        
        [HttpPost("search-with-model")]
        public async Task<ApiResponse<PagedResult<MongoDbEntity>>> Search([FromBody] SearchModel model)
        {

            var result =  await _mongoRepositoryService.SearchAsync(model);

            return result;
        }

        [HttpPost("search-with-keyvalues")]
        public async Task<ApiResponse<PagedResult<MongoDbEntity>>> SearchWithKeyValue([FromBody] List<KeyValuePair<string, string>> model)
        {
            var result = await _mongoRepositoryService.SearchAsync(model);
            return result;
        }

        [HttpPut("update-with-model")]
        public async Task<ApiResponse<bool>> Update([FromBody] MongoDbEntity model)
        {
            return await _mongoRepositoryService.Update(model);
        }

        [HttpPost("search-bsondocument")]
        public async Task<ApiResponse<PagedResult<string>>> SearchDynamic([FromBody] List<KeyValuePair<string, string>> model)
        {
            var bsonResult = await _mongoBsonRepositoryService.SearchBsonAsync(model);
            
            var pagedResult = PagedResult<string>.ToPagedList(
                bsonResult.Data.ResultSet.Select(x => x.ToJson()).ToList(), 
                bsonResult.Data.TotalCount, 
                bsonResult.Data.CurrentPage, 
                bsonResult.Data.PageSize);

            return await ApiResponseFactory.Success<PagedResult<string>>(pagedResult);

        }
    }
}