using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class ClubHistory
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("PlayerPositionId")]
        public int PlayerPositionId { get; set; }

        [BsonIgnoreIfNull]
        public PlayerPosition PlayerPosition { get; set; }

        [BsonElement("ClubName")]
        public string ClubName { get; set; }

        [BsonElement("League")]
        public string League { get; set; }

        [BsonElement("Region")]
        public string Region { get; set; }

        [BsonElement("StartDate")]
        public DateTime StartDate { get; set; }

        [BsonElement("EndDate")]
        public DateTime EndDate { get; set; }

        [BsonElement("AchievementsId")]
        public int AchievementsId { get; set; }

        [BsonIgnoreIfNull]
        public Achievements Achievements { get; set; }

        [BsonElement("PlayerId")]
        public string PlayerId { get; set; }

        [BsonIgnoreIfNull]
        public User Player { get; set; }
    }
}