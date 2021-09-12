using MongoRepository.Entities.Abstract;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MongoRepository.Entities.Concrete
{
    [BsonIgnoreExtraElements]
    public class MongoDbEntity : IEntity<string>
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        [BsonElement("_id", Order = 0)]
        public string Id { get; set; } // For autogenereate: ObjectId.GenerateNewId().ToString();

        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("Timestamp", Order = 101)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; 
    }
}