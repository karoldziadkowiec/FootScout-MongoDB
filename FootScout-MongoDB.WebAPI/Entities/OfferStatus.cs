using MongoDB.Bson.Serialization.Attributes;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class OfferStatus
    {
        [BsonId]
        public int Id { get; set; }
        [BsonElement("StatusName")]
        public string StatusName { get; set; }
    }
}