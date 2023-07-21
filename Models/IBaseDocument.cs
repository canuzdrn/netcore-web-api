using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace userMS.Models
{
    public interface IBaseDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime CreatedAt { get; }
    }
}
