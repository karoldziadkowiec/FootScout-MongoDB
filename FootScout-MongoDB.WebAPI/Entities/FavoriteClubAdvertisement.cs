using MongoDB.Bson.Serialization.Attributes;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class FavoriteClubAdvertisement
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("ClubAdvertisementId")]
        public int ClubAdvertisementId { get; set; }

        [BsonIgnoreIfNull]
        public ClubAdvertisement ClubAdvertisement { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonIgnoreIfNull]
        public virtual User User { get; set; }
    }
}