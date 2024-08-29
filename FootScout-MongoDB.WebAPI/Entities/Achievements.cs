using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class Achievements
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("NumberOfMatches")]
        public int NumberOfMatches { get; set; }

        [BsonElement("Goals")]
        public int Goals { get; set; }

        [BsonElement("Assists")]
        public int Assists { get; set; }

        [BsonElement("AdditionalAchievements")]
        [BsonIgnoreIfNull]
        public string AdditionalAchievements { get; set; }
    }
}