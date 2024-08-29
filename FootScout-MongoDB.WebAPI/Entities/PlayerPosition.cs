using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class PlayerPosition
    {
        [BsonId]
        public int Id { get; set; }
        [BsonElement("PositionName")]
        public string PositionName { get; set; }
    }
}
