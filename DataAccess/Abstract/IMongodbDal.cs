using MongoRepository.Entities.Concrete;

namespace MongoRepository.DataAccess.Abstract
{
    public interface IMongodbDal : IRepository<MongoDbEntity, string>
    {
    }
}