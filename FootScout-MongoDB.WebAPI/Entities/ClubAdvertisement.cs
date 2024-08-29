using MongoDB.Bson.Serialization.Attributes;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class ClubAdvertisement
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("PlayerPositionId")]
        public int PlayerPositionId { get; set; }

        [BsonIgnoreIfNull]
        public virtual PlayerPosition PlayerPosition { get; set; }

        [BsonElement("ClubName")]
        public string ClubName { get; set; }

        [BsonElement("League")]
        public string League { get; set; }

        [BsonElement("Region")]
        public string Region { get; set; }

        [BsonElement("SalaryRangeId")]
        public int SalaryRangeId { get; set; }

        [BsonIgnoreIfNull]
        public virtual SalaryRange SalaryRange { get; set; }

        [BsonElement("CreationDate")]
        public DateTime CreationDate { get; set; }

        [BsonElement("EndDate")]
        public DateTime EndDate { get; set; }

        [BsonElement("ClubMemberId")]
        public string ClubMemberId { get; set; }

        [BsonIgnoreIfNull]
        public virtual User ClubMember { get; set; }
    }
}