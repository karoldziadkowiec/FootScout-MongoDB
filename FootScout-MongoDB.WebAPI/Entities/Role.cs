using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class Role
    {
        [BsonId]
        public string Id { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
    }
}