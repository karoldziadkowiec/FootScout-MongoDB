using MongoDB.Bson.Serialization.Attributes;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class Message
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("ChatId")]
        public int ChatId { get; set; }

        [BsonIgnoreIfNull]
        public virtual Chat Chat { get; set; }

        [BsonElement("Content")]
        public string Content { get; set; }

        [BsonElement("SenderId")]
        public string SenderId { get; set; }

        [BsonIgnoreIfNull]
        public virtual User Sender { get; set; }

        [BsonElement("ReceiverId")]
        public string ReceiverId { get; set; }

        [BsonIgnoreIfNull]
        public virtual User Receiver { get; set; }

        [BsonElement("Timestamp")]
        public DateTime Timestamp { get; set; }
    }
}