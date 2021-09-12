using System;

namespace MongoRepository.Models
{
    public class SearchModel : PageDTO
    {
        public string Id { get; set; }
        public string MongoDatabaseName { get; set; }
        public string MongoDatabaseUrl { get; set; }
        public string MongoCollectionName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
