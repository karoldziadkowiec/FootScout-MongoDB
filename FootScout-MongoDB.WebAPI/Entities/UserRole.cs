using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class UserRole
    {
        [BsonId]
        public int Id { get; set; }
        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonIgnoreIfNull]
        public virtual User User { get; set; }
        [BsonElement("RoleId")]
        public string RoleId { get; set; }

        [BsonIgnoreIfNull]
        public virtual Role Role { get; set; }
    }
}