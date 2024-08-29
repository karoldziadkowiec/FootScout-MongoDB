using MongoDB.Bson.Serialization.Attributes;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class SalaryRange
    {
        [BsonId]
        public int Id { get; set; }
        [BsonElement("Min")]
        public double Min { get; set; }
        [BsonElement("Max")]
        public double Max { get; set; }
    }
}