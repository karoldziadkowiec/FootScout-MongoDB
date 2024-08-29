using MongoDB.Bson.Serialization.Attributes;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class ClubOffer
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("PlayerAdvertisementId")]
        public int PlayerAdvertisementId { get; set; }

        [BsonIgnoreIfNull]
        public virtual PlayerAdvertisement PlayerAdvertisement { get; set; }

        [BsonElement("OfferStatusId")]
        public int OfferStatusId { get; set; }

        [BsonIgnoreIfNull]
        public virtual OfferStatus OfferStatus { get; set; }

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

        [BsonElement("Salary")]
        public double Salary { get; set; }

        [BsonElement("AdditionalInformation")]
        [BsonIgnoreIfNull]
        public string AdditionalInformation { get; set; }

        [BsonElement("CreationDate")]
        public DateTime CreationDate { get; set; }

        [BsonElement("ClubMemberId")]
        public string ClubMemberId { get; set; }

        [BsonIgnoreIfNull]
        public virtual User ClubMember { get; set; }
    }
}