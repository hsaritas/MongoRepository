using Microsoft.Extensions.Configuration;
using MongoRepository.DataAccess.Abstract;
using MongoRepository.Entities.Concrete;

namespace MongoRepository.DataAccess.Concrete
{
    public class MongoDbModelDal : MongoDbRepositoryBase<MongoDbEntity>, IMongodbDal
    {
        public MongoDbModelDal(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
