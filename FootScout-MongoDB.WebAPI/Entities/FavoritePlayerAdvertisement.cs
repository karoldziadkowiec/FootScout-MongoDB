using MongoDB.Bson.Serialization.Attributes;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class FavoritePlayerAdvertisement
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("PlayerAdvertisementId")]
        public int PlayerAdvertisementId { get; set; }

        [BsonIgnoreIfNull]
        public PlayerAdvertisement PlayerAdvertisement { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonIgnoreIfNull]
        public virtual User User { get; set; }
    }
}