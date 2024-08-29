using MongoDB.Bson.Serialization.Attributes;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class PlayerFoot
    {
        [BsonId]
        public int Id { get; set; }
        [BsonElement("FootName")]
        public string FootName { get; set; }
    }
}