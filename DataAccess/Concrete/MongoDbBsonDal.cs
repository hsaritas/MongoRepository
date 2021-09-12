using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoRepository.DataAccess.Abstract;

namespace MongoRepository.DataAccess.Concrete
{
    public class MongoDbBsonDal : MongoDbBsonRepositoryBase<BsonDocument>
    {
        public MongoDbBsonDal(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
