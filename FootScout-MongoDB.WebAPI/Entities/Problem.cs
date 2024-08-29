using MongoDB.Bson.Serialization.Attributes;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class Problem
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("CreationDate")]
        public DateTime CreationDate { get; set; }

        [BsonElement("IsSolved")]
        public bool IsSolved { get; set; }

        [BsonElement("RequesterId")]
        public string RequesterId { get; set; }

        [BsonIgnoreIfNull]
        public virtual User Requester { get; set; }
    }
}