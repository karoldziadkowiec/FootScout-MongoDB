using MongoDB.Bson.Serialization.Attributes;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class Chat
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("User1Id")]
        public string User1Id { get; set; }

        [BsonIgnoreIfNull]
        public virtual User User1 { get; set; }

        [BsonElement("User2Id")]
        public string User2Id { get; set; }

        [BsonIgnoreIfNull]
        public virtual User User2 { get; set; }
    }
}