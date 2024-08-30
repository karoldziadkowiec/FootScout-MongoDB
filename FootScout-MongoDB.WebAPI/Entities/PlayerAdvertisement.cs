using MongoDB.Bson.Serialization.Attributes;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class PlayerAdvertisement
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("PlayerPositionId")]
        public int PlayerPositionId { get; set; }

        [BsonIgnoreIfNull]
        public virtual PlayerPosition PlayerPosition { get; set; }

        [BsonElement("League")]
        public string League { get; set; }

        [BsonElement("Region")]
        public string Region { get; set; }

        [BsonElement("Age")]
        public int Age { get; set; }

        [BsonElement("Height")]
        public int Height { get; set; }

        [BsonElement("PlayerFootId")]
        public int PlayerFootId { get; set; }

        [BsonIgnoreIfNull]
        public virtual PlayerFoot PlayerFoot { get; set; }

        [BsonElement("SalaryRangeId")]
        public int SalaryRangeId { get; set; }

        [BsonIgnoreIfNull]
        public virtual SalaryRange SalaryRange { get; set; }

        [BsonElement("CreationDate")]
        public DateTime CreationDate { get; set; }

        [BsonElement("EndDate")]
        public DateTime EndDate { get; set; }

        [BsonElement("PlayerId")]
        public string PlayerId { get; set; }

        [BsonIgnoreIfNull]
        public virtual User Player { get; set; }
    }
}