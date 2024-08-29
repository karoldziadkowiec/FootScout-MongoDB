using MongoDB.Bson.Serialization.Attributes;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class PlayerOffer
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("ClubAdvertisementId")]
        public int ClubAdvertisementId { get; set; }

        [BsonIgnoreIfNull]
        public virtual ClubAdvertisement ClubAdvertisement { get; set; }

        [BsonElement("OfferStatusId")]
        public int OfferStatusId { get; set; }

        [BsonIgnoreIfNull]
        public virtual OfferStatus OfferStatus { get; set; }

        [BsonElement("PlayerPositionId")]
        public int PlayerPositionId { get; set; }

        [BsonIgnoreIfNull]
        public virtual PlayerPosition PlayerPosition { get; set; }

        [BsonElement("Age")]
        public int Age { get; set; }

        [BsonElement("Height")]
        public int Height { get; set; }

        [BsonElement("PlayerFootId")]
        public int PlayerFootId { get; set; }

        [BsonIgnoreIfNull]
        public virtual PlayerFoot PlayerFoot { get; set; }

        [BsonElement("Salary")]
        public double Salary { get; set; }

        [BsonElement("AdditionalInformation")]
        [BsonIgnoreIfNull]
        public string AdditionalInformation { get; set; }

        [BsonElement("CreationDate")]
        public DateTime CreationDate { get; set; }

        [BsonElement("PlayerId")]
        public string PlayerId { get; set; }

        [BsonIgnoreIfNull]
        public virtual User Player { get; set; }
    }
}